using ECommerceModels;

namespace ECommerce.DTO
{
    public class SingleProductDTO : Product
    {
        public int? Count { get; set; }
        public double? Rating { get; set; }
    }
}
