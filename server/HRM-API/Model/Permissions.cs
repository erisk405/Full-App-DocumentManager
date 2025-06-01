using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRM_API.Model
{
    [Table("Permissions")]
    public class Permissions
    {
        [Key]
        public string PermissionId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public bool IsReadable { get; set; } = false;

        [Required]
        public bool IsWriteable { get; set; } = false;

        [Required]
        public bool IsDeleteable { get; set; } = false;

        // Optional: เชื่อมกลับไปยัง Role (1:1)
        [JsonIgnore]
        public Role? Role { get; set; }
    }
}
