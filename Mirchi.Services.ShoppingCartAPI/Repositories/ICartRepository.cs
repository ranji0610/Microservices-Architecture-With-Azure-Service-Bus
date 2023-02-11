using Mirchi.Services.ShoppingCartAPI.Models.DTOs;

namespace Mirchi.Services.ShoppingCartAPI.Repositories
{
    public interface ICartRepository
    {
        Task<CartDTO> GetCartByUserId(string userId);
        Task<CartDTO> CreateUpdateCart(CartDTO cartDTO);
        Task<bool> RemoveFromCart(int cartDetailsId);
        Task<bool> ClearCart(string userId);
    }
}
