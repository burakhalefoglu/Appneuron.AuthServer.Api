using System;
using System.Collections.Generic;
using System.Text;

namespace Business.MessageBrokers.Kafka.Model
{
    public class ProjectCreationResult
    {
        public string Accesstoken { get; set; }
        public int UserId { get; set; }
    }
}
