using Microsoft.AspNetCore.Mvc;
using Mirchi.Services.CouponAPI.Models.Dtos;
using Mirchi.Services.CouponAPI.Repositories;

namespace Mirchi.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;
        protected ResponseDTO _response;

        public CouponAPIController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
            _response = new ResponseDTO();
        }

        [HttpGet]
        [Route("{code}")]
        public async Task<object> GetCouponByCode(string code)
        {
            try
            {
                var coupon = await _couponRepository.GetCouponByCode(code);
                _response.Result = coupon;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }
    }
}

