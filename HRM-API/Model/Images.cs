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

        public string? ProfileImagePath { get; set; }

        public string? ProfileImageFileName { get; set; }

        public DateTime? ProfileImageUploadedAt { get; set; }

        // เชื่อมกลับไปยัง User (One-to-One)
        [JsonIgnore]
        public User? User { get; set; }
    }
}
