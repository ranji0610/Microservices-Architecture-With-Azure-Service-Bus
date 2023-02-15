using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Mirchi.MessageBus
{
    public class AzureServiceBusMessageBus : IMessageBus
    {
        private readonly string connectionString = "Endpoint=sb://mirchirestaurant.servicebus.windows.net/;SharedAccessKeyName=default;SharedAccessKey=oPXcymqutuV3tuC/zdKrfmWUbDN/VnNYU+ASbJLdb2s=;";
        public async Task PublishMessage(BaseMessage message, string topicName)
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
