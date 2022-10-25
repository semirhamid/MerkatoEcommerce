namespace ECommerce.DTO.Responses
{
    public class UserProfile : UserManagerResponse
    {
        public string? LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string[]? userRoles { get; set; }
    }
}
