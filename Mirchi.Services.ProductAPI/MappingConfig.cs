using AutoMapper;
using Mirchi.Services.ProductAPI.Models;
using Mirchi.Services.ProductAPI.Models.DTOs;

namespace Mirchi.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Product, ProductDTO>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
