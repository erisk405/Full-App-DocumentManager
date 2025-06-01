using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRM_API.Model
{
    public class Document
    {
        [Key]
        public string DocumentId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Filename { get; set; } = string.Empty;

        [Required]
        public string Original_name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        public string FileType { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = string.Empty;

        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CreatedByUser")]
        public string? CreatedByUserId { get; set; }

        public User? CreatedByUser { get; set; } // เอกสารถูกสร้างโดยใคร

        public bool IsDelete { get; set; }
    }

}
