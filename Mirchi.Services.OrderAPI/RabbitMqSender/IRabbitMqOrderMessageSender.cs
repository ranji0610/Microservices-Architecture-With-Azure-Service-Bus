using Mirchi.MessageBus;

namespace Mirchi.Services.OrderAPI.RabbitMqSender
{
    public interface IRabbitMqOrderMessageSender
    {
        void SendMessage(BaseMessage message, string queueName);
    }
}
