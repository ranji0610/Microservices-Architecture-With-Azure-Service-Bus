using Microsoft.AspNetCore.Mvc;
using Mirchi.MessageBus;
using Mirchi.Services.ShoppingCartAPI.Messages;
using Mirchi.Services.ShoppingCartAPI.Models;
using Mirchi.Services.ShoppingCartAPI.Models.DTOs;
using Mirchi.Services.ShoppingCartAPI.Repositories;

namespace Mirchi.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        protected ResponseDTO _response;
        private readonly IMessageBus _messageBus;
        private readonly ICouponRepository _couponRepository;

        public CartAPIController(ICartRepository cartRepository, IMessageBus messageBus, ICouponRepository couponRepository)
        {
            _cartRepository = cartRepository;
            _response = new ResponseDTO();
            _messageBus = messageBus;
            _couponRepository = couponRepository;
        }

        [HttpGet]
        [Route("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                var cart = await _cartRepository.GetCartByUserId(userId);
                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost]
        [Route("addcart")]
        public async Task<object> AddCart(CartDTO cartDTO)
        {
            try
            {
                var cart = await _cartRepository.CreateUpdateCart(cartDTO);
                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost]
        [Route("updatecart")]
        public async Task<object> UpdateCart(CartDTO cartDTO)
        {
            try
            {
                var cart = await _cartRepository.CreateUpdateCart(cartDTO);
                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost]
        [Route("removecart")]
        public async Task<object> RemoveCart([FromBody]int cartDetailsId)
        {
            try
            {
                bool isSuccess = await _cartRepository.RemoveFromCart(cartDetailsId);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost]
        [Route("clearcart")]
        public async Task<object> ClearCart([FromBody] string userId)
        {
            try
            {
                bool isSuccess = await _cartRepository.ClearCart(userId);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost]
        [Route("applycoupon")]
        public async Task<object> ApplyCoupon(CartDTO cartDTO)
        {
            try
            {
                bool isSuccess = await _cartRepository.ApplyCoupon(cartDTO.CartHeader.UserId, cartDTO.CartHeader.CouponCode);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost]
        [Route("removecoupon")]
        public async Task<object> RemoveCoupon([FromBody] string userId)
        {
            try
            {
                bool isSuccess = await _cartRepository.RemoveCoupon(userId);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost]
        [Route("checkout")]
        public async Task<object> Checkout(CheckoutHeaderDto checkoutHeader)
        {
            try
            {
                CartDTO cart = await _cartRepository.GetCartByUserId(checkoutHeader.UserId);
                if(cart == null)
                {
                    return BadRequest();
                }

                if(!string.IsNullOrWhiteSpace(checkoutHeader.CouponCode))
                {
                    var coupon = await _couponRepository.GetCouponAsync(checkoutHeader.CouponCode);
                    if(coupon != null && checkoutHeader.DiscountTotal != coupon.DiscountAmount)
                    {
                        _response.IsSuccess = false;
                        _response.ErrorMessages = new List<string> { "Coupon price has change , please confirm" };
                        _response.DisplayMessage = "Coupon price has change , please confirm";
                        return _response;
                    }
                }
                checkoutHeader.CartDetails = cart.CartDetails;
                await _messageBus.PublishMessage(checkoutHeader, "checkoutmessage");
                //bool isSuccess = await _cartRepository.Checkout(checkoutHeader);
                //_response.Result = isSuccess;
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
