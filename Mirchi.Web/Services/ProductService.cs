﻿using Mirchi.Web.Models;
using Mirchi.Web.Services.IServices;

namespace Mirchi.Web.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<T> CreateProductAsync<T>(ProductDto productDto)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = productDto,
                ApiUrl = SD.ProductAPIBase + "api/products",
                AccessToken = string.Empty
            });
        }

        public async Task<T> DeleteProductAsync<T>(int productId)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.DELETE,
                ApiUrl = SD.ProductAPIBase + "api/products/" + productId,
                AccessToken = string.Empty
            });
        }

        public async Task<T> GetAllProductsAsync<T>()
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                ApiUrl = SD.ProductAPIBase + "api/products",
                AccessToken = string.Empty
            });
        }

        public async Task<T> GetProductByIdAsync<T>(int productId)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                ApiUrl = SD.ProductAPIBase + "api/products/" + productId,
                AccessToken = string.Empty
            });
        }

        public async Task<T> UpdateProductAsync<T>(ProductDto productDto)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = productDto,
                ApiUrl = SD.ProductAPIBase + "api/products",
                AccessToken = string.Empty
            });
        }
    }
}
