using System;

namespace Business.MessageBrokers.Models
{
    public class CreateClientMessageComamnd
    {
        public long ClientId { get; set; }
        public long ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPaidClient { get; set; }
    }
}