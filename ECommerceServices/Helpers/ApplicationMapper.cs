using AutoMapper;
using ECommerceModels;


namespace ECommerceServices.Helpers
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<Product, Product>().ReverseMap();
            CreateMap<Order, Order>().ReverseMap();
        }
    }
}


