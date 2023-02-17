using Mirchi.Services.OrderAPI.Messages;
using Mirchi.Services.OrderAPI.Models;
using Mirchi.Services.OrderAPI.RabbitMqSender;
using Mirchi.Services.OrderAPI.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace Mirchi.Services.OrderAPI.Messaging
{
    public class RabbitMqCheckoutConsumer : BackgroundService
    {
        private readonly IOrderRepository _orderRepository;
        private IConnection _connection;
        private IModel _channel;
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private readonly IRabbitMqOrderMessageSender _rabbitMqOrderMessageSender;
        public RabbitMqCheckoutConsumer(IOrderRepository orderRepository, IRabbitMqOrderMessageSender rabbitMqOrderMessageSender)
        {
            _orderRepository = orderRepository;
            _hostName = "localhost";
            _password = "guest";
            _userName = "guest";
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "checkoutqueue", false, false, false, arguments: null);
            _rabbitMqOrderMessageSender = rabbitMqOrderMessageSender;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if(stoppingToken.IsCancellationRequested)
            {
                return Task.CompletedTask;
            }

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var checkoutHeader = JsonConvert.DeserializeObject<CheckoutHeaderDto>(content);
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
                _rabbitMqOrderMessageSender.SendMessage(paymentRequestMessage, "orderqueue");
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("checkoutqueue", false, consumer);
            return Task.CompletedTask;
        }
    }
}
