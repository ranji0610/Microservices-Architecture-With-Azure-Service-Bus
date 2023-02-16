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
        private readonly ICouponService _couponService;

        public CartController(ICartService cartService, IProductService productService, ICouponService couponService)
        {
            _cartService = cartService;
            _productService = productService;
            _couponService = couponService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await LoadCartDtoFromLoggedInUser());
        }

        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoFromLoggedInUser());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartDTO cartDTO)
        {
            try
            {
                var userId = User.Claims.Where(claim => claim.Type == "sub")?.FirstOrDefault()?.Value;
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _cartService.Checkout<ResponseDTO>(cartDTO.CartHeader, accessToken);
                if (response != null && !response.IsSuccess)
                {
                    TempData["Error"] = response.DisplayMessage;
                    return RedirectToAction(nameof(Checkout));
                }
                return RedirectToAction("Confirmation");
            }
            catch (Exception)
            {
                return View(cartDTO);
            }
        }

        public async Task<IActionResult> Confirmation()
        {
            return View();
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(CartDTO cartDTO)
        {
            var userId = User.Claims.Where(claim => claim.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.ApplyCouponAsync<ResponseDTO>(cartDTO, accessToken);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon(CartDTO cartDTO)
        {
            var userId = User.Claims.Where(claim => claim.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveCouponAsync<ResponseDTO>(userId, accessToken);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View();
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

            if (cartDTO.CartHeader != null)
            {
                if (!string.IsNullOrWhiteSpace(cartDTO.CartHeader.CouponCode))
                {
                    var couponResponse = await _couponService.GetCouponAsync<ResponseDTO>(cartDTO.CartHeader.CouponCode, accessToken);
                    if (couponResponse != null && couponResponse.IsSuccess)
                    {
                        var coupon = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(couponResponse.Result));                        
                        cartDTO.CartHeader.DiscountTotal = coupon.DiscountAmount;
                    }
                }
                cartDTO.CartDetails.ToList().ForEach(item =>
                {
                    cartDTO.CartHeader.OrderTotal += (item.Product.Price * item.Count);
                });

                cartDTO.CartHeader.OrderTotal -= cartDTO.CartHeader.DiscountTotal;
            }

            return cartDTO;
        }
    }
}
