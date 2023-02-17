using Mirchi.MessageBus;

namespace Mirchi.Services.ShoppingCartAPI.RabbitMqSender
{
    public interface IRabbitMqCartMessageSender
    {
        void SendMessage(BaseMessage message, string queueName);
    }
}
