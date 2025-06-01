using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRM_API.Model
{
    public class Role
    {
        [Key]
        public string RoleId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string RoleName { get; set; } = string.Empty;

        // ความสัมพันธ์ 1:1 กับ Permission
        [ForeignKey("Permission")]
        public string PermissionId { get; set; } = Guid.NewGuid().ToString(); // หรือจะให้ nullable ก็ได้

        public Permissions? Permission { get; set; }

        [JsonIgnore]
        public ICollection<User>? Users { get; set; }
    }
}
