using Mirchi.Services.OrderAPI.RabbitMqSender;
using Mirchi.Services.PaymentAPI.Messages;
using Newtonsoft.Json;
using PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mirchi.Services.PaymentAPI.Messaging
{
    public class RabbitMqOrderConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private readonly IProcessPayment _processPayment;
        private readonly IRabbitMqPaymentMessageSender _rabbitMqPaymentMessageSender;
        public RabbitMqOrderConsumer(IProcessPayment processPayment, IRabbitMqPaymentMessageSender rabbitMqPaymentMessageSender)
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
            _channel.QueueDeclare(queue: "orderqueue", false, false, false, arguments: null);
            _processPayment = processPayment;
            _rabbitMqPaymentMessageSender = rabbitMqPaymentMessageSender;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                return Task.CompletedTask;
            }

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var paymentObj = JsonConvert.DeserializeObject<PaymentRequestMessage>(content);
                var result = _processPayment.PaymentProcessor();
                UpdatePaymentResultMessage updatePaymentResultMessage = new()
                {
                    Status = result,
                    OrderId = paymentObj.OrderId,
                    Email = paymentObj.Email
                };

                try
                {
                    _rabbitMqPaymentMessageSender.SendMessage(updatePaymentResultMessage, "paymentqueue");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }


                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("orderqueue", false, consumer);
            return Task.CompletedTask;
        }
    }
}
