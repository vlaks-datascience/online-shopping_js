using AutoMapper;
using back_shopping.DTOs;
using back_shopping.Models;

namespace back_shopping.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<OrderProduct, OrderProductDTO>().ReverseMap();
        }

    }
}
