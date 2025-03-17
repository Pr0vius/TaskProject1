using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ms.rabbitmq.Events;
using RabbitMQ.Client;
using System.Text;

namespace ms.rabbitmq.Producers
{
    public class EventProducer : IProducer
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EventProducer> _logger;
        public EventProducer(IConfiguration configuration, ILogger<EventProducer> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Produce(RabbitMqEvent rabbitMqEvent)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _configuration.GetSection("Communications:EventBus:HostName").Value!,
                };
                using (var connection = await factory.CreateConnectionAsync())
                using (var channel = await connection.CreateChannelAsync())
                {
                    var queue = rabbitMqEvent.GetType().Name;
                    await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    var body = Encoding.UTF8.GetBytes(rabbitMqEvent.Serialize());

                    _logger.LogTrace($"Producing event to queue: {queue}");
                    await channel.BasicPublishAsync(
                        exchange: string.Empty,
                        routingKey: queue,
                        body: body);
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cant Produce the event");
            }
        }
    }
}
