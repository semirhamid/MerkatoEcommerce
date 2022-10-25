namespace ECommerce.DTO.Responses
{
    public class UserManagerResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public DateTime? ExpireDate { get; set; }
        public IList<string>? Role { get; set; }
        public string? FirstName { get; set; }
        public string? UserId { get; set; }
    }
}
