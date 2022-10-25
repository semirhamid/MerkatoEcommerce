using AutoMapper;
using ECommerce.DTO;
using ECommerceModels;


namespace ECommerceServices.Helpers
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<Product, Product>().ReverseMap();
            CreateMap<Order, Order>().ReverseMap();
            CreateMap<SingleProductDTO, Product>().ReverseMap();
            CreateMap<ProductRating, Product>().ReverseMap();
        }
    }
}


