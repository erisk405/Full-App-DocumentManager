namespace HRM_API.DTOs
{
    public class RoleWithPermissionDto
    {
        public string? RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public PermissionDto? Permission { get; set; }

    }

    public class PermissionDto
    {
        public string? PermissionId { get; set; }
        public bool IsReadable { get; set; }
        public bool IsWriteable { get; set; }
        public bool IsDeleteable { get; set; }
    }


}
