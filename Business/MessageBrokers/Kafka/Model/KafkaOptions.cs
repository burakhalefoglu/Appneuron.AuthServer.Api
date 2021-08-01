using System;
using System.Collections.Generic;
using System.Text;

namespace Business.MessageBrokers.Kafka.Model
{
    public class KafkaOptions
    {
        public string HostName { get; set; }
        public int Port { get; set; }
    }
}
