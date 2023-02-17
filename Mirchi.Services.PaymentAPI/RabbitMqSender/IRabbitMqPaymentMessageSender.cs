using Mirchi.MessageBus;

namespace Mirchi.Services.OrderAPI.RabbitMqSender
{
    public interface IRabbitMqPaymentMessageSender
    {
        void SendMessage(BaseMessage message, string queueName);
    }
}
