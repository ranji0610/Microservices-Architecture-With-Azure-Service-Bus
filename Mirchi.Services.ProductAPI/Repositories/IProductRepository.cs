using Mirchi.Services.ProductAPI.Models.DTOs;

namespace Mirchi.Services.ProductAPI.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDTO>> GetProducts();

        Task<ProductDTO> GetProductById(int productId);

        Task<ProductDTO> CreateUpdateProduct(ProductDTO productDTO);
        Task<bool> DeleteProduct(int productId);
    }
}
