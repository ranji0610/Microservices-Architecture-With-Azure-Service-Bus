using Microsoft.AspNetCore.Mvc;
using Mirchi.Services.ProductAPI.Models.DTOs;
using Mirchi.Services.ProductAPI.Repositories;

namespace Mirchi.Services.ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        protected ResponseDTO _response;

        private readonly IProductRepository _productRepository;
        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _response = new ResponseDTO();
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                var products = await _productRepository.GetProducts();
                _response.Result = products;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpGet]
        [Route("{productId}")]
        public async Task<object> Get(int productId)
        {
            try
            {
                var product = await _productRepository.GetProductById(productId);
                _response.Result = product;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPost]
        public async Task<object> Post([FromBody] ProductDTO productDTO)
        {
            try
            {
                _response.Result = await _productRepository.CreateUpdateProduct(productDTO);                
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpPut]
        public async Task<object> Put([FromBody] ProductDTO productDTO)
        {
            try
            {
                _response.Result = await _productRepository.CreateUpdateProduct(productDTO);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }

        [HttpDelete]
        [Route("{productId}")]
        public async Task<object> Delete(int productId)
        {
            try
            {
                _response.Result = await _productRepository.DeleteProduct(productId);
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
