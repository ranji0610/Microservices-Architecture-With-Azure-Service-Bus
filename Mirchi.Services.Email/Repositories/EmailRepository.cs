using Microsoft.EntityFrameworkCore;
using Mirchi.Services.Email.DBContexts;
using Mirchi.Services.Email.Messages;
using Mirchi.Services.Email.Models;

namespace Mirchi.Services.Email.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<ApplicationDBContext> _dbContextOptions;

        public EmailRepository(DbContextOptions<ApplicationDBContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task SendAndLogEmail(UpdatePaymentResultMessage updatePaymentResultMessage)
        {
            EmailLog emailLog = new()
            {
                Email = updatePaymentResultMessage.Email,
                EmailSent = DateTime.Now,
                Log = $"Order - {updatePaymentResultMessage.OrderId} created succesfully"
            };

            await using var _db = new ApplicationDBContext(_dbContextOptions);
            _db.EmailLogs.Add(emailLog);
            await _db.SaveChangesAsync();
        }
    }
}
