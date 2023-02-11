using Microsoft.AspNetCore.Mvc;
using Mirchi.Services.ShoppingCartAPI.Models;
using Mirchi.Services.ShoppingCartAPI.Models.DTOs;
using Mirchi.Services.ShoppingCartAPI.Repositories;

namespace Mirchi.Services.ShoppingCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        protected ResponseDTO _response;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
            _response = new ResponseDTO();
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
    }
}
