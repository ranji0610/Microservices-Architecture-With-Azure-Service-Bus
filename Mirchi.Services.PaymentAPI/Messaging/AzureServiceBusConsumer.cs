using Azure.Messaging.ServiceBus;
using Mirchi.MessageBus;
using Mirchi.Services.PaymentAPI.Messages;
using Newtonsoft.Json;
using PaymentProcessor;
using System.Text;

namespace Mirchi.Services.PaymentAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {        
        private readonly string ServiceBusConnectionString;
        private readonly string OrderPaymentTopicName;
        private readonly string OrderPaymentSubscriptionName;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor OrderPaymentProcessor;
        private readonly IMessageBus _messageBus;
        private readonly IProcessPayment _processPayment;

        public AzureServiceBusConsumer(IConfiguration configuration, IMessageBus messageBus, IProcessPayment processPayment)
        {            
            _configuration = configuration;
            _messageBus = messageBus;
            ServiceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            OrderPaymentTopicName = _configuration.GetValue<string>("OrderPaymentTopicName");
            OrderPaymentSubscriptionName = _configuration.GetValue<string>("OrderPaymentSubscriptionName");            
            var client = new ServiceBusClient(ServiceBusConnectionString);
            OrderPaymentProcessor = client.CreateProcessor(OrderPaymentTopicName, OrderPaymentSubscriptionName);
            _processPayment= processPayment;
        }

        public async Task Start()
        {
            OrderPaymentProcessor.ProcessMessageAsync += OnProcessPaymentMessageReceived;
            OrderPaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await OrderPaymentProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            //if (!checkoutProcessor.IsProcessing)
            //{
            //    await checkoutProcessor.StopProcessingAsync();
            //    await checkoutProcessor.DisposeAsync();
            //}
        }

        private async Task OnProcessPaymentMessageReceived(ProcessMessageEventArgs processMessageEventArgs)
        {
            var message = processMessageEventArgs.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            var paymentObj = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);
            var result = _processPayment.PaymentProcessor();
            UpdatePaymentResultMessage updatePaymentResultMessage = new()
            {
                Status = result,
                OrderId = paymentObj.OrderId
            };

            try
            {
                var connectionString = _configuration.GetValue<string>("ServiceBusConnectionStringForOrderUpdateTopic");
                var topicName = _configuration.GetValue<string>("OrderPaymentUpdateResultTopicName");
                await _messageBus.PublishMessage(updatePaymentResultMessage, topicName, connectionString);
                await processMessageEventArgs.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs processErrorEventArgs)
        {
            Console.WriteLine(processErrorEventArgs.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
