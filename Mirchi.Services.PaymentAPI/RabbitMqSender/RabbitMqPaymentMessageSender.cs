using Mirchi.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mirchi.Services.OrderAPI.RabbitMqSender
{
    public class RabbitMqPaymentMessageSender : IRabbitMqPaymentMessageSender
    {
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private IConnection _connection;
        private const string ExchangeName = "DirectPaymentUpdate_Exchange";
        private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
        private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";

        public RabbitMqPaymentMessageSender()
        {
            _hostName = "localhost";
            _password = "guest";
            _userName = "guest";
        }
        public void SendMessage(BaseMessage message, string queueName)
        {
            if (ConnectionExists() == false)
                CreateConnection();

            using var channel = _connection.CreateModel();
            channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct, durable: false);
            channel.QueueDeclare(PaymentOrderUpdateQueueName, false, false, false, null);
            channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
            channel.QueueBind(PaymentEmailUpdateQueueName, exchange: ExchangeName, routingKey: "PaymentEmail", null);
            channel.QueueBind(PaymentOrderUpdateQueueName, exchange: ExchangeName, routingKey: "PaymentOrder", null);
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange: ExchangeName, routingKey: "PaymentEmail", basicProperties: null, body: body);
            channel.BasicPublish(exchange: ExchangeName, routingKey: "PaymentOrder", basicProperties: null, body: body);
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _hostName,
                    UserName = _userName,
                    Password = _password
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private bool ConnectionExists()
        {
            return _connection != null;
        }
    }
}
