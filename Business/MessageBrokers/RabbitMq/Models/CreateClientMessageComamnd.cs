using System;

namespace Business.MessageBrokers.RabbitMq.Models
{
    public class CreateClientMessageComamnd
    {
        public string ClientId { get; set; }
        public string ProjectKey { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPaidClient { get; set; }
    }
}