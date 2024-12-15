using System.Text;
using System.Text.Json;
using kursah_5semestr.Abstractions;
using RabbitMQ.Client;

namespace kursah_5semestr.Services
{
    public class BrokerService : IBrokerService
    {
        private ConnectionFactory _connectionFactory;

        public BrokerService()
        {
            _connectionFactory = new ConnectionFactory {  HostName = "localhost" };
        }

        public async Task SendMessage(string exchange, object message)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(exchange: exchange, type: ExchangeType.Fanout);
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            await channel.BasicPublishAsync(exchange: exchange, routingKey: "", body: body);
        }
    }
}
