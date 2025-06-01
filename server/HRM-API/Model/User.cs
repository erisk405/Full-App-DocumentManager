using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRM_API.Model
{
    public class User
    {
        [Key]
        public string UserId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string First_name { get; set; } = string.Empty;

        [Required]
        public string Last_name { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [ForeignKey("Role")]
        public string RoleId { get; set; } = string.Empty;
        public Role? Role { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        public ICollection<Document>? Documents { get; set; }

        // เพิ่ม foreign key สำหรับรูปภาพ
        [ForeignKey("Image")]
        public string? ImageId { get; set; }
        public Images? Image { get; set; }
    }

}
