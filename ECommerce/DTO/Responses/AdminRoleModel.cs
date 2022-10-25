namespace ECommerce.DTO.Responses
{
    public class AdminRoleModel
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
