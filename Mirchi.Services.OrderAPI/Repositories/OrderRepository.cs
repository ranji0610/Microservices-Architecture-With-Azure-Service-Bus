using Microsoft.EntityFrameworkCore;
using Mirchi.Services.OrderAPI.DBContexts;
using Mirchi.Services.OrderAPI.Models;

namespace Mirchi.Services.OrderAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<ApplicationDBContext> _dbContextOptions;

        public OrderRepository(DbContextOptions<ApplicationDBContext> dbContextOptions)
        {
            _dbContextOptions= dbContextOptions;
        }

        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            await using var _db = new ApplicationDBContext(_dbContextOptions);
            await _db.OrderHeaders.AddAsync(orderHeader);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid)
        {
            await using var _db = new ApplicationDBContext(_dbContextOptions);
            var orderHeaderDB = await _db.OrderHeaders.FirstOrDefaultAsync(orderHeader => orderHeader.OrderHeaderId == orderHeaderId);
            if (orderHeaderDB != null)
            {
                orderHeaderDB.PaymentStatus= paid;
                await _db.SaveChangesAsync();
            }
        }
    }
}
