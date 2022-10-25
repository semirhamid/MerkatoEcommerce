using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public class ProductUpdateModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public double Weight { get; set; }
        public int Quantity { get; set; }
        public string? Category { get; set; }

        public string? Color { get; set; }
        public string? Size { get; set; }
        public IFormFile? PhotoPath { get; set; }
    }
}
