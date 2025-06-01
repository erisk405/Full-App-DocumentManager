using System.ComponentModel.DataAnnotations;

namespace HRM_API.DTOs
{
    public class UserUpdateDTO
    {
        public string First_name { get; set; } = string.Empty;
        public string Last_name { get; set; } = string.Empty;
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string RoleId { get; set; }
        // ไม่บังคับให้มีรูปโปรไฟล์
        public IFormFile? ProfileImage { get; set; }
    }
}
