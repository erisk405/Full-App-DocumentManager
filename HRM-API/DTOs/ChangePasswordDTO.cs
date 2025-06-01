namespace HRM_API.DTOs
{
    public class ChangePasswordDTO
    {
        public string UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
