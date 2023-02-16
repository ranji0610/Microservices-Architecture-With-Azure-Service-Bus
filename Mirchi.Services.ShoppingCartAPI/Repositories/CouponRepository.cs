using Mirchi.Services.ShoppingCartAPI.Models.DTOs;
using Newtonsoft.Json;

namespace Mirchi.Services.ShoppingCartAPI.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient _httpClient;

        public CouponRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CouponDTO> GetCouponAsync(string couponName)
        {
            var response = await _httpClient.GetAsync($"api/coupon/{ couponName}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
            if(responseObj != null && responseObj.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDTO>(responseObj.Result.ToString());
            }

            return new CouponDTO();
        }
    }
}
