using Mirchi.Services.CouponAPI.Models.Dtos;

namespace Mirchi.Services.CouponAPI.Repositories
{
    public interface ICouponRepository
    {
        Task<CouponDTO> GetCouponByCode(string code);
    }
}
