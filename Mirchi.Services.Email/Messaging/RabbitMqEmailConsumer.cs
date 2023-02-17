using Mirchi.Services.Email.Messages;
using Mirchi.Services.Email.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mirchi.Services.Email.Messaging
{
    public class RabbitMqEmailConsumer : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private const string ExchangeName = "DirectPaymentUpdate_Exchange";
        private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
        private readonly IEmailRepository _emailRepository;        
        public RabbitMqEmailConsumer(IEmailRepository emailRepository)
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
            _channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
            _channel.QueueBind(PaymentEmailUpdateQueueName, exchange: ExchangeName, routingKey: "PaymentEmail", arguments: null);
            _emailRepository = emailRepository;
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
                var responseObj = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(content);
                await _emailRepository.SendAndLogEmail(responseObj);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(PaymentEmailUpdateQueueName, false, consumer);
            return Task.CompletedTask;
        }
    }
}
