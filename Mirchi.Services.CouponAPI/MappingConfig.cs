using AutoMapper;
using Mirchi.Services.CouponAPI.Models;
using Mirchi.Services.CouponAPI.Models.Dtos;

namespace Mirchi.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Coupon, CouponDTO>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
