using System.ComponentModel.DataAnnotations;

namespace ECommerceModels
{
    public class OrderModel
    {
        public int Id { get; set; }
        [Required]
        public int Product { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "The User Id is required")]
        public string UserId { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        public string City { get; set; }
        public int? ZipCode { get; set; }

    }
}
