using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceModels
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public virtual Product Product { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required(ErrorMessage ="The User Id is required")]
        public string UserId { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        public string City { get; set; }
        public string Status { get; set; }
        public double TotalPrice { get; set; }
        public int? ZipCode { get; set; }
        public string? TrackingId { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? Date { get; set; }
        public string? Address { get; set; }

    }
}
