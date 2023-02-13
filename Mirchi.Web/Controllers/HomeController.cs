using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mirchi.Web.Models;
using Mirchi.Web.Services.IServices;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mirchi.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> products = new();
            var response = await _productService.GetAllProductsAsync<ResponseDTO>("");
            if (response != null && response.IsSuccess)
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(JsonConvert.SerializeObject(response.Result));
            }
            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            ProductDto product = new();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetProductByIdAsync<ResponseDTO>(productId, accessToken);
            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(JsonConvert.SerializeObject(response.Result));
            }
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult Login()
        {
            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        [HttpPost]
        [ActionName("Details")]
        [Authorize]
        public async Task<IActionResult> DetailsPost(ProductDto productDto)
        {
            CartDTO cart = new();
            CartHeaderDTO cartHeader = new()
            {
                UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
            };
            CartDetailsDTO cartDetails = new()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId
            };
            var response = await _productService.GetProductByIdAsync<ResponseDTO>(productDto.ProductId, string.Empty);
            if (response != null && response.IsSuccess)
            {
                cartDetails.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            List<CartDetailsDTO> cartDetailsDTOs = new()
            {
                cartDetails
            };
            cart.CartDetails = cartDetailsDTOs;
            cart.CartHeader = cartHeader;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var addResponse = await _cartService.AddToCartAsync<ResponseDTO>(cart, accessToken);
            if (addResponse != null && addResponse.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return View(productDto);
        }
    }
}