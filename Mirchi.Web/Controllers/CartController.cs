using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Mirchi.Web.Models;
using Mirchi.Web.Services.IServices;
using Newtonsoft.Json;

namespace Mirchi.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await LoadCartDtoFromLoggedInUser());
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(claim => claim.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveFromCartAsync<ResponseDTO>(cartDetailsId, accessToken);
            
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        private async Task<CartDTO> LoadCartDtoFromLoggedInUser()
        {
            var userId = User.Claims.Where(claim => claim.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.GetCartByUserIdAsync<ResponseDTO>(userId, accessToken);
            CartDTO cartDTO = new();
            if (response != null && response.IsSuccess)
            {
                cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
            }

            if(cartDTO.CartHeader != null)
            {
                cartDTO.CartDetails.ToList().ForEach(item =>
                {
                    cartDTO.CartHeader.OrderTotal += (item.Product.Price * item.Count);
                });
            }

            return cartDTO;
        }
    }
}
