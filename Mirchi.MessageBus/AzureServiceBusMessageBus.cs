using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Mirchi.MessageBus
{
    public class AzureServiceBusMessageBus : IMessageBus
    {        
        public async Task PublishMessage(BaseMessage message, string topicName, string connectionString)
        {
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender serviceBusSender = client.CreateSender(topicName);            
            var jsonMessage = JsonConvert.SerializeObject(message);
            var finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await serviceBusSender.SendMessageAsync(finalMessage);            
            await client.DisposeAsync();
        }
    }
}
