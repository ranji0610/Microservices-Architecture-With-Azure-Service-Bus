using Microsoft.AspNetCore.Mvc;
using Mirchi.Web.Models;
using Mirchi.Web.Services.IServices;
using Newtonsoft.Json;

namespace Mirchi.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> productDtos = new();
            var response = await _productService.GetAllProductsAsync<ResponseDTO>();
            if (response != null && response.IsSuccess)
            {
                productDtos = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(productDtos);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto model)
        {
            if(ModelState.IsValid)
            {
                var response = await _productService.CreateProductAsync<ResponseDTO>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
            }
            
            return View(model);
        }

        public async Task<IActionResult> Edit(int productId)
        {
            var response = await _productService.GetProductByIdAsync<ResponseDTO>(productId);
            var productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            return View(productDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductDto model)
        {
            if(ModelState.IsValid)
            {
                var response = await _productService.UpdateProductAsync<ResponseDTO>(model);
                if(response != null && response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }

                return View();
            }

            return View();
        }

        public async Task<IActionResult> Delete(int productId)
        {
            var response = await _productService.GetProductByIdAsync<ResponseDTO>(productId);
            var productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            return View(productDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.DeleteProductAsync<ResponseDTO>(model.ProductId);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }

                return View();
            }

            return View();
        }
    }
}
