namespace SwiftServe_API.DTOs
{
    public class CreateNotificationDto
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
