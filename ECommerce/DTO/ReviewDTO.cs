namespace ECommerce.DTO
{
    public class ReviewDTO
    {
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public double Rating { get; set; }
        public string? Comment { get; set; }

    }
}
