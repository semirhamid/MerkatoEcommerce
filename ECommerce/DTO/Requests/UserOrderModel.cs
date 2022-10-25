using ECommerceModels;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO.Requests
{
    public class UserOrderModel
    {
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "The User Id is required")]
        public string Country { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string? Address { get; set; }
        public int ZipCode { get; set; }

    }
}
