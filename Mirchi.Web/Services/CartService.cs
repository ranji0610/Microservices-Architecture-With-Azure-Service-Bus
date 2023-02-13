using Mirchi.Web.Models;
using Mirchi.Web.Services.IServices;

namespace Mirchi.Web.Services
{
    public class CartService : BaseService, ICartService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CartService(IHttpClientFactory httpClientFactory): base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
             
        public async Task<T> AddToCartAsync<T>(CartDTO cartDTO, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDTO,
                ApiUrl = SD.ShoppingCartAPIBase + "api/cart/addcart",
                AccessToken = token
            });
        }

        public async Task<T> GetCartByUserIdAsync<T>(string userId, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                ApiUrl = SD.ShoppingCartAPIBase + "api/cart/GetCart/" + userId,
                AccessToken = token
            });
        }

        public async Task<T> RemoveFromCartAsync<T>(int cartDetailsId, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDetailsId,
                ApiUrl = SD.ShoppingCartAPIBase + "api/cart/removecart",
                AccessToken = token
            });
        }

        public async Task<T> UpdateCartAsync<T>(CartDTO cartDTO, string token = null)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDTO,
                ApiUrl = SD.ShoppingCartAPIBase + "api/cart/updatecart",
                AccessToken = token
            });
        }
    }
}
