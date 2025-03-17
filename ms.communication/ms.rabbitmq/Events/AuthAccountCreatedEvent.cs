using System.Text.Json;

namespace ms.rabbitmq.Events
{
    public class AuthAccountCreatedEvent : RabbitMqEvent
    {
        public Guid EventId = Guid.NewGuid();

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Serialize() => JsonSerializer.Serialize(this);
    }
}
