using Mirchi.MessageBus;

namespace Mirchi.Services.PaymentAPI.Messages
{
    public class UpdatePaymentResultMessage: BaseMessage
    {
        public int OrderId { get; set; }
        public bool Status { get; set; }
    }
}
