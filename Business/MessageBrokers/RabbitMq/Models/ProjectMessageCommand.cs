using System;
using System.Collections.Generic;
using System.Text;

namespace Business.MessageBrokers.RabbitMq.Models
{
    public class ProjectMessageCommand
    {
        public int UserId { get; set; }
        public string ProjectKey { get; set; }
    }
}
