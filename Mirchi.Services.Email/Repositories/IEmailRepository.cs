using Mirchi.Services.Email.Messages;
using Mirchi.Services.Email.Models;

namespace Mirchi.Services.Email.Repositories
{
    public interface IEmailRepository
    {
        Task SendAndLogEmail(UpdatePaymentResultMessage updatePaymentResultMessage);
    }
}
