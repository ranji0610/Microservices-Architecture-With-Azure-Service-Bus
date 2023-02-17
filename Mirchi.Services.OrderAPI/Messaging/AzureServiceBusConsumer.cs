using Azure.Messaging.ServiceBus;
using Mirchi.MessageBus;
using Mirchi.Services.OrderAPI.Messages;
using Mirchi.Services.OrderAPI.Models;
using Mirchi.Services.OrderAPI.Repositories;
using Newtonsoft.Json;
using System.Text;

namespace Mirchi.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IOrderRepository _orderRepository;
        private readonly string ServiceBusConnectionString;
        private readonly string CheckoutMessageTopic;
        private readonly string CheckoutSubscriptionName;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor checkoutProcessor;
        private readonly IMessageBus _messageBus;
        private readonly string OrderPaymentTopicName;
        private readonly string OrderPaymentResultTopicName;
        private readonly string OrderPaymentResultSubscriptionName;
        private readonly string ServiceBusConnectionStringForOrderPaymentResultTopic;
        private ServiceBusProcessor OrderServiceBusProcessor;

        public AzureServiceBusConsumer(IOrderRepository orderRepository, IConfiguration configuration
            ,IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;
            _messageBus = messageBus;
            ServiceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            CheckoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            CheckoutSubscriptionName = _configuration.GetValue<string>("CheckoutSubscriptionName");
            OrderPaymentTopicName = _configuration.GetValue<string>("OrderPaymentTopicName");            
            var client = new ServiceBusClient(ServiceBusConnectionString);
            checkoutProcessor = client.CreateProcessor(CheckoutMessageTopic);
            OrderPaymentResultTopicName = _configuration.GetValue<string>("OrderPaymentResultTopicName");
            OrderPaymentResultSubscriptionName = _configuration.GetValue<string>("OrderPaymentResultSubscriptionName");
            ServiceBusConnectionStringForOrderPaymentResultTopic = _configuration.GetValue<string>("ServiceBusConnectionStringForOrderPaymentResultTopic");
            var orderClient = new ServiceBusClient(ServiceBusConnectionStringForOrderPaymentResultTopic);
            OrderServiceBusProcessor = orderClient.CreateProcessor(OrderPaymentResultTopicName, OrderPaymentResultSubscriptionName);
        }

        public async Task Start()
        {
            checkoutProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
            checkoutProcessor.ProcessErrorAsync +=  ErrorHandler;
            await checkoutProcessor.StartProcessingAsync();

            OrderServiceBusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateMessageReceived;
            OrderServiceBusProcessor.ProcessErrorAsync += ErrorHandler;
            await OrderServiceBusProcessor.StartProcessingAsync();
        }

        private async Task OnOrderPaymentUpdateMessageReceived(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            var updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);
            await _orderRepository.UpdateOrderPaymentStatus(updatePaymentResultMessage.OrderId, updatePaymentResultMessage.Status);
            await arg.CompleteMessageAsync(arg.Message);
        }

        public async Task Stop()
        {
            //if (!checkoutProcessor.IsProcessing)
            //{
            //    await checkoutProcessor.StopProcessingAsync();
            //    await checkoutProcessor.DisposeAsync();
            //}
        }

        private async Task OnCheckoutMessageReceived(ProcessMessageEventArgs processMessageEventArgs)
        {
            var message = processMessageEventArgs.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            var checkoutHeader = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);
            OrderHeader orderHeader = new()
            {
                UserId = checkoutHeader.UserId,
                CardNumber = checkoutHeader.CardNumber,
                CartTotalItems = checkoutHeader.CartTotalItems,
                CouponCode = checkoutHeader.CouponCode,
                CVV = checkoutHeader.CVV,
                DiscountTotal = checkoutHeader.DiscountTotal,
                Email = checkoutHeader.Email,
                ExpiryMonthYear = checkoutHeader.ExpiryMonthYear,
                FirstName = checkoutHeader.FirstName,
                LastName = checkoutHeader.LastName,
                OrderTotal = checkoutHeader.OrderTotal,
                Phone = checkoutHeader.Phone,
                PickUpDateTime = checkoutHeader.PickUpDateTime,
                OrderTime = DateTime.Now,
                OrderDetails = new List<OrderDetails>(),
                PaymentStatus = false
            };

            foreach (var cartDetail in checkoutHeader.CartDetails)
            {
                OrderDetails orderDetails = new()
                {
                    Count = cartDetail.Count,
                    ProductId = cartDetail.ProductId,
                    ProductName = cartDetail.Product.Name,
                    ProductPrice = cartDetail.Product.Price
                };
                orderHeader.CartTotalItems += cartDetail.Count;
                orderHeader.OrderDetails.Add(orderDetails);
            }

            await _orderRepository.AddOrder(orderHeader);

            PaymentRequestMessage paymentRequestMessage = new()
            {
                Name = orderHeader.FirstName + " " + orderHeader.LastName,
                CardNumber = orderHeader.CardNumber,
                OrderId = orderHeader.OrderHeaderId,
                CVV = orderHeader.CVV,
                ExpiryMonthYear = orderHeader.ExpiryMonthYear,
                OrderTotal = orderHeader.OrderTotal,
                Email = orderHeader.Email
            };

            try
            {
                var connectionString = _configuration.GetValue<string>("ServiceBusConnectionStringForOrderTopic");
                await _messageBus.PublishMessage(paymentRequestMessage, OrderPaymentTopicName, connectionString);
                await processMessageEventArgs.CompleteMessageAsync(message);
            }
            catch(Exception ex)
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
