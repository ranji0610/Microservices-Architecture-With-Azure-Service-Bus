using Mirchi.Services.OrderAPI.Models;

namespace Mirchi.Services.OrderAPI.Repositories
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(OrderHeader orderHeader);

        Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid);
    }
}
