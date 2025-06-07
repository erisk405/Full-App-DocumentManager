using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRM_API.Model
{
    [Table("Images")]
    public class Images
    {
        [Key]
        public string ImageId { get; set; } = Guid.NewGuid().ToString();

        public string? PublicId { get; set; }             // ใช้สำหรับลบรูป
        public string? ImageUrl { get; set; }             // URL จาก Cloudinary
        public string? FileName { get; set; }             // ชื่อไฟล์เดิม (optional)
        public DateTime? UploadedAt { get; set; }         // วันเวลาที่อัปโหลด

        // ความสัมพันธ์กับ User
        [JsonIgnore]
        public User? User { get; set; }
    }

}
