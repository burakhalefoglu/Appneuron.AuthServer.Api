using System;
using System.Linq;
using Business.MessageBrokers.Kafka.Model;
using Confluent.Kafka;
using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business.MessageBrokers.Kafka
{
    public static class KafkaAdminHelper
    {
        public static int SetPartitionCountAsync(string topicName)
        {
            var Configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
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