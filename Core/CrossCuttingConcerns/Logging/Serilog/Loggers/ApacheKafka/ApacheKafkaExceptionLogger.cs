using Serilog;
using Serilog.Sinks.Kafka;
using Core.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka
{
    public class ApacheKafkaExceptionLogger : BaseKafkaLogger
    {
        public ApacheKafkaExceptionLogger():base(0)
        {
          
        }
    }
}
