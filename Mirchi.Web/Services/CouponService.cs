using Mirchi.Web.Models;
using Mirchi.Web.Services.IServices;

namespace Mirchi.Web.Services
{
    public class CouponService : BaseService, ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CouponService(IHttpClientFactory httpClientFactory):base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<T> GetCouponAsync<T>(string couponCode, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                ApiUrl = SD.CouponAPIBase + "api/coupon/" + couponCode,
                AccessToken = token
            });
        }
    }
}
