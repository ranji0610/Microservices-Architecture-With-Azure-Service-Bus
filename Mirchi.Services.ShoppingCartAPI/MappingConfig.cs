using AutoMapper;
using Mirchi.Services.ShoppingCartAPI.Models;
using Mirchi.Services.ShoppingCartAPI.Models.DTOs;

namespace Mirchi.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Product, ProductDTO>().ReverseMap();
                config.CreateMap<Cart, CartDTO>().ReverseMap();
                config.CreateMap<CartHeader, CartHeaderDTO>().ReverseMap();
                config.CreateMap<CartDetails, CartDetailsDTO>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
