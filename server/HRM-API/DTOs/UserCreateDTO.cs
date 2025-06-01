using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HRM_API.DTOs
{
    public class UserCreateDTO
    {
        [Required(ErrorMessage = "First name is required")]
        public string First_name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        public string Last_name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string RoleId { get; set; } = string.Empty;

        // ไม่บังคับให้มีรูปโปรไฟล์
        public IFormFile? ProfileImage { get; set; }
    }
}