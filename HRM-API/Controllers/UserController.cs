using System.Reflection.Metadata;
using System.Security.Claims;
using HRM_API.DTOs;
using HRM_API.Model;
using HRM_API.Repository;
using HRM_API.Services;
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
        private readonly CloudinaryService _cloudinaryService;
        public UserController(UserRepository userRepository,ImageRepository imageRepository, IWebHostEnvironment webHostEnvironment, CloudinaryService cloudinaryService)
        {
            this.userRepository = userRepository;
            this.imageRepository = imageRepository;
            this.webHostEnvironment = webHostEnvironment;
            this._cloudinaryService = cloudinaryService;
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

        //[Authorize]
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
                    // อัปโหลดขึ้น Cloudinary
                    var result = await _cloudinaryService.UploadImageAsync(userDTO.ProfileImage, "profiles");

                    var image = new Images
                    {
                        ImageId = Guid.NewGuid().ToString(),
                        PublicId = result.PublicId,
                        ImageUrl = result.SecureUrl,
                        FileName = userDTO.ProfileImage.FileName,
                        UploadedAt = DateTime.UtcNow
                    };

                    user.Image = image;
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
                        ImageUrl = user.Image?.ImageUrl
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
            try // ✨ เพิ่ม try-catch block
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
                    // 🔥 ลบรูปเก่าจาก Cloudinary
                    if (user.Image != null && !string.IsNullOrEmpty(user.Image.PublicId))
                    {
                        await _cloudinaryService.DeleteImageAsync(user.Image.PublicId);
                        await imageRepository.Delete(user.Image.ImageId); // ลบจาก DB
                    }

                    // 📤 อัปโหลดใหม่
                    var result = await _cloudinaryService.UploadImageAsync(dto.ProfileImage, "profiles");

                    var image = new Images
                    {
                        ImageId = Guid.NewGuid().ToString(),
                        PublicId = result.PublicId,
                        ImageUrl = result.SecureUrl,
                        FileName = dto.ProfileImage.FileName,
                        UploadedAt = DateTime.UtcNow
                    };

                    user.Image = image;
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
                        ImageUrl = user.Image?.ImageUrl
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating user: {ex.Message}");
            }
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


        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userRepository.GetUserImage(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            // เช็ครูปโปรไฟล์แล้วลบ ถ้ามี
            if (user.Image != null && !string.IsNullOrEmpty(user.Image.ImageUrl))
            {
                // 🔥 ลบรูปเก่าจาก Cloudinary
                if (user.Image != null && !string.IsNullOrEmpty(user.Image.PublicId))
                {
                    await _cloudinaryService.DeleteImageAsync(user.Image.PublicId);
                    await imageRepository.Delete(user.Image.ImageId); // ลบจาก DB
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
