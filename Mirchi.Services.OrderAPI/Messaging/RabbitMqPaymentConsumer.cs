using Mirchi.Services.OrderAPI.Messages;
using Mirchi.Services.OrderAPI.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mirchi.Services.OrderAPI.Messaging
{
    public class RabbitMqPaymentConsumer : BackgroundService
    {
        private readonly IOrderRepository _orderRepository;
        private IConnection _connection;
        private IModel _channel;
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private const string ExchangeName = "DirectPaymentUpdate_Exchange";
        private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";
        private readonly string queueName = string.Empty;
        public RabbitMqPaymentConsumer(IOrderRepository orderRepository)
        {
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
            _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct, durable: false);
            _channel.QueueDeclare(PaymentOrderUpdateQueueName, false, false, false, null);
            _channel.QueueBind(PaymentOrderUpdateQueueName, exchange: ExchangeName, routingKey: "PaymentOrder", arguments: null);
            _orderRepository = orderRepository;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                return Task.CompletedTask;
            }

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(content);
                await _orderRepository.UpdateOrderPaymentStatus(updatePaymentResultMessage.OrderId, updatePaymentResultMessage.Status);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(PaymentOrderUpdateQueueName, false, consumer);
            return Task.CompletedTask;
        }
    }
}
