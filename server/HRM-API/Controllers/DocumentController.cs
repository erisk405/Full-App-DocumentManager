using System.Net;
using System.Security.Claims;
using CloudinaryDotNet;
using HRM_API.Model;
using HRM_API.Repository;
using HRM_API.Services;
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
        private readonly CloudinaryService _cloudinaryService;

        public DocumentController(DocumentRepository documentRepository ,IWebHostEnvironment webHostEnvironment, CloudinaryService cloudinaryService)
        {
            this.doc = documentRepository;
            this.webHostEnvironment = webHostEnvironment;
            this._cloudinaryService = cloudinaryService;
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
        public async Task<IActionResult> UploadFile(
            IFormFile file,
            [FromForm] string filename,
            [FromForm] string description,
            [FromForm] string status)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Token does not contain user ID");

            string userId = userIdClaim.Value;

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadResult = await _cloudinaryService.UploadFileUniqueAsync(file, "documents");

            var extension = Path.GetExtension(file.FileName);
            var sanitizedFilename = Path.GetFileNameWithoutExtension(filename);
            var newFilename = sanitizedFilename + extension;

            var document = new Document
            {
                Filename = newFilename,
                Original_name = file.FileName,
                FilePath = uploadResult.SecureUrl,
                FileType = file.ContentType,
                Description = description,
                Status = status,
                CreateAt = DateTime.UtcNow,
                CreatedByUserId = userId,
                PublicId = uploadResult.PublicId
            };

            await doc.Add(document);

            return Ok(new
            {
                message = "Uploaded successfully",
                url = uploadResult.SecureUrl
            });
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

            try
            {
                // ✅ ดึง publicId 
                var cloudinaryPublicId = document.PublicId!;

                // ✅ ลบไฟล์จาก Cloudinary
                await _cloudinaryService.DeleteFileAsync(cloudinaryPublicId);

                // ✅ ลบข้อมูลจากฐานข้อมูล
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
                return NotFound("Document not found");

            var fileUrl = document.FilePath;
            Console.WriteLine(fileUrl);
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(fileUrl);

            Console.WriteLine(response);
            if (!response.IsSuccessStatusCode)
                return NotFound("File not found on Cloudinary");

            var stream = await response.Content.ReadAsStreamAsync();
            var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            memory.Position = 0;

            // ✅ ต้องระบุ Content-Type และชื่อไฟล์
            return File(memory, "application/pdf", document.Original_name);
        }

    }
}
