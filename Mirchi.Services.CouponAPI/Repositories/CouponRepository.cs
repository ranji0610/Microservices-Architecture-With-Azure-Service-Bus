using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mirchi.Services.CouponAPI.DBContexts;
using Mirchi.Services.CouponAPI.Models.Dtos;

namespace Mirchi.Services.CouponAPI.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;
        private readonly IMapper _mapper;

        public CouponRepository(ApplicationDBContext applicationDBContext, IMapper mapper)
        {
            _applicationDBContext = applicationDBContext;
            _mapper = mapper;   
        }

        public async Task<CouponDTO> GetCouponByCode(string code)
        {
            return _mapper.Map<CouponDTO>(await _applicationDBContext.Coupons.FirstOrDefaultAsync(coupon => coupon.CouponCode.Equals(code)));
        }
    }
}
