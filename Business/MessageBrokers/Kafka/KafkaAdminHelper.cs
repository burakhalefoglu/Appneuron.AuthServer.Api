using Confluent.Kafka;
using Core.Utilities.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Business.MessageBrokers.Kafka.Model;

namespace Business.MessageBrokers.Kafka
{
    public static class KafkaAdminHelper
    {

        public static int SetPartitionCountAsync(string topicName)
        {
            IConfiguration Configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
            var kafkaOptions = Configuration.GetSection("ApacheKafka").Get<KafkaOptions>();

            using (var adminClient = new AdminClientBuilder(new AdminClientConfig 
            { BootstrapServers = $"{kafkaOptions.HostName}:{kafkaOptions.Port}" }).Build())
            {

                var meta = adminClient.GetMetadata(TimeSpan.FromSeconds(20));

                return meta.Topics.Find(p => p.Topic == topicName).Partitions.Count();

            }

        }
    }
}