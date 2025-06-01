namespace HRM_API.DTOs
{
    public class RoleResponseDto
    {
        public string? RoleName { get; set; }
        public string? PermissionId { get; set; }
        public bool IsReadable { get; set; }
        public bool IsWriteable { get; set; }
        public bool IsDeleteable { get; set; }
    }
}
