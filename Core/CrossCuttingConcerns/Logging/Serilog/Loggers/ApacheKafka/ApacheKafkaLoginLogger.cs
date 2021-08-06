using Serilog;
using Serilog.Sinks.Kafka;
using Core.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka
{
    public class ApacheKafkaLoginLogger : BaseKafkaLogger
    {
        public ApacheKafkaLoginLogger():base(1)
        {
          
        }
    }
}
