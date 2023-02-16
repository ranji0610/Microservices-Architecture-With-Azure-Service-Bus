using Mirchi.Services.ShoppingCartAPI.Models.DTOs;

namespace Mirchi.Services.ShoppingCartAPI.Repositories
{
    public interface ICouponRepository
    {
        Task<CouponDTO> GetCouponAsync(string couponName);
    }
}
