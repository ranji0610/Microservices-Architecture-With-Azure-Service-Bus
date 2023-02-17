using Azure.Messaging.ServiceBus;
using Mirchi.Services.Email.Messages;
using Mirchi.Services.Email.Repositories;
using Newtonsoft.Json;
using System.Text;

namespace Mirchi.Services.Email.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IConfiguration _configuration;        
        private readonly string OrderPaymentResultTopicName;
        private readonly string OrderPaymentResultSubscriptionName;
        private readonly string ServiceBusConnectionStringForOrderPaymentResultTopic;
        private ServiceBusProcessor EmailServiceBusProcessor;

        public AzureServiceBusConsumer(IEmailRepository emailRepository, IConfiguration configuration)
        {
            _emailRepository = emailRepository;
            _configuration = configuration;            
            OrderPaymentResultTopicName = _configuration.GetValue<string>("OrderPaymentResultTopicName");
            OrderPaymentResultSubscriptionName = _configuration.GetValue<string>("OrderPaymentResultSubscriptionName");
            ServiceBusConnectionStringForOrderPaymentResultTopic = _configuration.GetValue<string>("ServiceBusConnectionStringForOrderPaymentResultTopic");
            var orderClient = new ServiceBusClient(ServiceBusConnectionStringForOrderPaymentResultTopic);
            EmailServiceBusProcessor = orderClient.CreateProcessor(OrderPaymentResultTopicName, OrderPaymentResultSubscriptionName);
        }

        public async Task Start()
        {
            EmailServiceBusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateMessageReceived;
            EmailServiceBusProcessor.ProcessErrorAsync += ErrorHandler;
            await EmailServiceBusProcessor.StartProcessingAsync();
        }

        private async Task OnOrderPaymentUpdateMessageReceived(ProcessMessageEventArgs arg)
        {
            try
            {
                var message = arg.Message;
                var body = Encoding.UTF8.GetString(message.Body);
                var responseObj = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);
                await _emailRepository.SendAndLogEmail(responseObj);
                await arg.CompleteMessageAsync(arg.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task Stop()
        {
            //if (!checkoutProcessor.IsProcessing)
            //{
            //    await checkoutProcessor.StopProcessingAsync();
            //    await checkoutProcessor.DisposeAsync();
            //}
        }

        private Task ErrorHandler(ProcessErrorEventArgs processErrorEventArgs)
        {
            Console.WriteLine(processErrorEventArgs.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
