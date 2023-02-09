using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Mirchi.Services.ProductAPI.DBContexts;
using Mirchi.Services.ProductAPI.Models;
using Mirchi.Services.ProductAPI.Models.DTOs;

namespace Mirchi.Services.ProductAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;
        private readonly IMapper _mapper;

        public ProductRepository(ApplicationDBContext applicationDBContext, IMapper mapper)
        {
            _applicationDBContext = applicationDBContext;
            _mapper = mapper;
        }
        public async Task<ProductDTO> CreateUpdateProduct(ProductDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            if(product.ProductId > 0)
            {
                _applicationDBContext.Products.Update(product);
            }
            else
            {
                await _applicationDBContext.Products.AddAsync(product);
            }

            await _applicationDBContext.SaveChangesAsync();

            return productDTO;
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            try
            {
                return await _applicationDBContext.Products.Where(x => x.ProductId == productId).ExecuteDeleteAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ProductDTO> GetProductById(int productId)
        {
            return _mapper.Map<ProductDTO>(await _applicationDBContext.Products.Where(x => x.ProductId == productId).FirstOrDefaultAsync());
        }

        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            return _mapper.Map<List<ProductDTO>>(await _applicationDBContext.Products.ToListAsync());
        }
    }
}
