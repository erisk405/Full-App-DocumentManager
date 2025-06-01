namespace HRM_API.DTOs
{
    public class UserGetDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string First_name { get; set; } = string.Empty;
        public string Last_name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string Role_name { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;

        public PermissionDto? Permission { get; set; } // ✅ เพิ่มตรงนี้
        public DateTime CreateAt { get; set; }
        public DateTime? ProfileImageUploadedAt { get; set; } 
    }
}
