using System.Security.Claims;
using HRM_API.Model;
using HRM_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentRepository doc;
        private readonly IWebHostEnvironment webHostEnvironment;

        public DocumentController(DocumentRepository documentRepository ,IWebHostEnvironment webHostEnvironment)
        {
            this.doc = documentRepository;
            this.webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<ActionResult> DocumentList()
        {
            var allDocument = await doc.GetAllDocument();
            return Ok(allDocument);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Document>> GetById(string id)
        {
            var document = await doc.GetById(id);
            if (document == null)
            {
                return NotFound();
            }
            return Ok(document);
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string filename, [FromForm] string description, [FromForm] string status)
        {
            // ดึง userId จาก Claims ใน token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Token does not contain user ID");

            string userId = userIdClaim.Value;

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // ✅ ใช้ชื่อไฟล์ที่ผู้ใช้กรอกมา แล้วเพิ่มนามสกุลตามไฟล์จริง
            var extension = Path.GetExtension(file.FileName); // เช่น .pdf
            var sanitizedFilename = Path.GetFileNameWithoutExtension(filename); // กัน user ส่งชื่อแปลก ๆ
            var newFilename = sanitizedFilename + extension;

            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadFolder);
            var filePath = Path.Combine(uploadFolder, newFilename);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"uploads/{newFilename}";

            var document = new Document
            {
                Filename = newFilename,
                Original_name = file.FileName,
                FilePath = relativePath,
                FileType = file.ContentType,
                Description = description,
                Status = status,
                CreateAt = DateTime.UtcNow,
                CreatedByUserId = userId // ✅ อาจรับจาก token แทน hardcode
            };

            await doc.Add(document);
            return Ok(new { message = "Uploaded successfully", path = relativePath });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocument(string id, [FromForm] string filename, [FromForm] string description, [FromForm] string status)
        {
            var document = await doc.GetById(id);
            if (document == null)
            {
                return NotFound("Document not found");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token.");
            }

            // Update only the metadata
            document.Filename = filename + Path.GetExtension(document.Filename); // Keep the original extension
            document.Description = description;
            document.Status = status;

            try
            {
                await doc.Update(document);
                return Ok(new
                {
                    message = "Document updated successfully",
                    document = new
                    {
                        documentId = document.DocumentId,
                        filename = document.Filename,
                        description = document.Description,
                        status = document.Status
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating document: {ex.Message}");
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(string id)
        {
            var document = await doc.GetById(id);
            if (document == null)
            {
                return NotFound("Document not found.");
            }

            // ดึง userId จาก Claims ใน token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Invalid token.");
            }

            // ตรวจสอบว่า user ที่ลบเป็นคนอัปโหลดไฟล์นี้หรือไม่
            //if (document.CreatedByUserId != userId)
            //{
            //    return Forbid("You are not authorized to delete this document.");
            //}

            // กำหนด path ที่ปลอดภัย
            var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
            var filePath = Path.GetFullPath(Path.Combine(uploadsFolder, document.Filename));

            // ป้องกัน Path Traversal
            if (!filePath.StartsWith(uploadsFolder))
            {
                return BadRequest("Invalid file path.");
            }

            try
            {
                // ลบไฟล์หากมี
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // ลบข้อมูลในฐานข้อมูล
                await doc.Delete(id);

                return Ok(new
                {
                    message = "Document deleted successfully.",
                    deletedId = id,
                    deletedFilename = document.Filename
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting document: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadDocument(string id)
        {
            var document = await doc.GetById(id);
            if (document == null)
            {
                return NotFound("Document not found");
            }

            var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
            var filePath = Path.GetFullPath(Path.Combine(uploadsFolder, document.Filename));

            if (!filePath.StartsWith(uploadsFolder))
            {
                return BadRequest("Invalid file path");
            }

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found on server");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, document.FileType, document.Original_name);
        }
    }
}
