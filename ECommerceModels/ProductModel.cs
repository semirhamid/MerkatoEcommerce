using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ECommerceModels
{
    public class ProductModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [MinLength(10)]
        public string Description { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public double Weight { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string Category { get; set; }

        public string Color { get; set; }
        public string Size { get; set; }
        public IFormFile PhotoPath { get; set; }
        
    }
}
