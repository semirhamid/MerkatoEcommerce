using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ECommerceModels
{
    public class Product
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
        public string PhotoPath { get; set; }
        public DateTime Date { get; set; }
        public string? SellerFirstName { get; set; }
        public string? SellerLastName { get; set; }
        public string? SellerEmail { get; set; }
        public string? SellerId { get; set; }
    }
}
