namespace Mirchi.MessageBus
{
    public interface IMessageBus
    {
        Task PublishMessage(BaseMessage message, string topicName, string connectionString);
    }
}
