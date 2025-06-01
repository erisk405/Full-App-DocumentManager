using System.Reflection.Metadata;
using System.Security.Claims;
using HRM_API.DTOs;
using HRM_API.Model;
using HRM_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserRepository userRepository;
        private readonly ImageRepository imageRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        public UserController(UserRepository userRepository,ImageRepository imageRepository, IWebHostEnvironment webHostEnvironment)
        {
            this.userRepository = userRepository;
            this.imageRepository = imageRepository;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
            public async Task<ActionResult> UserList(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? orderBy = null,
            [FromQuery] string? orderDirection = null)
        {
            var (users, totalCount) = await userRepository.GetAllUsers(pageNumber, pageSize, search, orderBy, orderDirection);

            var response = new
            {
                Users = users,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }

        //[HttpGet]
        //public async Task<ActionResult> UserList()
        //{
        //    var allUsers = await userRepository.GetAllUsers();
        //    return Ok(allUsers);
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(string id)
        {
            var user = await userRepository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserGetDTO>> GetMe()
        {
            // ดึง userId จาก Claims ใน token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Token does not contain user ID");

            string userId = userIdClaim.Value;

            var user = await userRepository.GetById(userId);
            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddUser([FromForm] UserCreateDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var isDuplicate = await userRepository.IsDuplicateEmailOrUsername(userDTO.Email, userDTO.Username);
            if (isDuplicate)
                return BadRequest("Email or Username already exists.");
            try
            {
                var user = new User
                {
                    First_name = userDTO.First_name,
                    Last_name = userDTO.Last_name,
                    Email = userDTO.Email,
                    Phone = userDTO.Phone,
                    Username = userDTO.Username,
                    Password = userDTO.Password,
                    RoleId = userDTO.RoleId,
                    CreateAt = DateTime.UtcNow
                };
                if (userDTO.ProfileImage != null && userDTO.ProfileImage.Length > 0)
                {
                    var uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "profiles");
                    Directory.CreateDirectory(uploadFolder);

                    var extension = Path.GetExtension(userDTO.ProfileImage.FileName);
                    var newFilename = $"{userDTO.Username}_{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadFolder, newFilename);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await userDTO.ProfileImage.CopyToAsync(stream);
                    }

                    var relativePath = $"/profiles/{newFilename}";

                    var image = new Images
                    {
                        ImageId = Guid.NewGuid().ToString(),
                        ProfileImagePath = relativePath,
                        ProfileImageFileName = userDTO.ProfileImage.FileName,
                        ProfileImageUploadedAt = DateTime.UtcNow
                    };

                    user.Image = image; // 👈 เชื่อมกับ user
                }

                await userRepository.Add(user);
                return Ok(new
                {
                    message = "User created successfully",
                    user = new
                    {
                        user.UserId,
                        user.Username,
                        user.Email,
                        ImagePath = user.Image?.ProfileImagePath
                    }
                });

            }
            catch (Exception ex)
            {

                // ถ้ามีข้อผิดพลาดเกิดขึ้น
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error : {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromForm] UserUpdateDTO dto)
        {
            var user = await userRepository.GetUserImage(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            // อัปเดตข้อมูลทั่วไป
            user.First_name = dto.First_name;
            user.Last_name = dto.Last_name;
            user.Email = dto.Email;
            user.Phone = dto.Phone;
            user.Username = dto.Username;
            user.RoleId = dto.RoleId;

            // ถ้ามีการส่งรูปมาใหม่
            if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
            {
                var uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "profiles");
                Directory.CreateDirectory(uploadFolder);

                // 🔥 ลบรูปเก่า
                if (user.Image != null && !string.IsNullOrEmpty(user.Image.ProfileImagePath))
                {
                    var oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, user.Image.ProfileImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    if (!string.IsNullOrEmpty(user.ImageId)) // Ensure ImageId is not null
                    {
                        await imageRepository.Delete(user.ImageId); // ลบ record รูปในฐานข้อมูล
                    }
                }

                // 📸 เพิ่มรูปใหม่
                var extension = Path.GetExtension(dto.ProfileImage.FileName);
                var newFilename = $"{dto.Username}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadFolder, newFilename);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ProfileImage.CopyToAsync(stream);
                }
                var relativePath = $"/profiles/{newFilename}";
                var image = new Images
                {
                    ImageId = Guid.NewGuid().ToString(),
                    ProfileImagePath = relativePath,
                    ProfileImageFileName = dto.ProfileImage.FileName,
                    ProfileImageUploadedAt = DateTime.UtcNow
                };

                user.Image = image; // 👈 เชื่อมกับ user
            }

            await userRepository.Update(user);
            return Ok(new
            {
                message = "User updated successfully",
                user = new
                {
                    user.UserId,
                    user.Username,
                    user.Email,
                    ImagePath = user.Image?.ProfileImagePath
                }
            });
        }


        [Authorize]
        [HttpPut("password")]
        public async Task<IActionResult> ChangeUserPassword([FromForm] ChangePasswordDTO dto)
        {
            var result = await userRepository.ChangePassword(dto.UserId, dto.CurrentPassword, dto.NewPassword);
           
            if (!result)
            {
                return BadRequest("รหัสผ่านเดิมไม่ถูกต้องหรือผู้ใช้ไม่พบ");
            }

           return Ok(new { message = "เปลี่ยนรหัสผ่านสำเร็จ" });
        }


        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userRepository.GetUserImage(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            // เช็ครูปโปรไฟล์แล้วลบ ถ้ามี
            if (user.Image != null && !string.IsNullOrEmpty(user.Image.ProfileImagePath))
            {
                // ลบไฟล์จากโฟลเดอร์
                var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "profiles");
                var imageFilename = Path.GetFileName(user.Image.ProfileImagePath); // ตัด directory ออกให้เหลือแค่ชื่อไฟล์
                var filePath = Path.GetFullPath(Path.Combine(uploadsFolder, imageFilename));

                // ป้องกัน path traversal
                if (!filePath.StartsWith(uploadsFolder))
                {
                    return BadRequest("Invalid file path.");
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                // ลบข้อมูลรูปภาพจากฐานข้อมูล
                if (!string.IsNullOrEmpty(user.ImageId))
                {
                    await imageRepository.Delete(user.ImageId);
                }
            }
            // ลบ user
            await userRepository.Delete(id);
            return Ok(new { message = "User and profile image deleted successfully." });
        }
    }
}
