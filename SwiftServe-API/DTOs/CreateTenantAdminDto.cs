namespace SwiftServe_API.DTOs
{
    public class CreateTenantAdminDto
    {
        public string TenantName { get; set; }

        public string AdminName { get; set; }
        public string AdminEmail { get; set; }
        public string AdminPhone { get; set; }
        public string Password { get; set; }
    }
}
