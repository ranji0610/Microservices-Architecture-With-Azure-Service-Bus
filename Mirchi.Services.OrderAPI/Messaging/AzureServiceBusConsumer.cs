using Azure.Messaging.ServiceBus;
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

        public AzureServiceBusConsumer(IOrderRepository orderRepository, IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;
            ServiceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            CheckoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
            CheckoutSubscriptionName = _configuration.GetValue<string>("CheckoutSubscriptionName");
            var client = new ServiceBusClient(ServiceBusConnectionString);
            checkoutProcessor = client.CreateProcessor(CheckoutMessageTopic,CheckoutSubscriptionName);
        }

        public async Task Start()
        {
            checkoutProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
            checkoutProcessor.ProcessErrorAsync +=  ErrorHandler;
            await checkoutProcessor.StartProcessingAsync();
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
        }

        private Task ErrorHandler(ProcessErrorEventArgs processErrorEventArgs)
        {
            Console.WriteLine(processErrorEventArgs.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
