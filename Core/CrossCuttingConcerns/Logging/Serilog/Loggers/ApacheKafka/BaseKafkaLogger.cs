using Serilog;
using Serilog.Sinks.Kafka;
using Core.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka
{
    public abstract class BaseKafkaLogger : LoggerServiceBase 
    {
        public BaseKafkaLogger(int index)
        {
            var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
            var logConfig = configuration.GetSection("SeriLogConfigurations:ApacheKafkaConfiguration")
               .Get<ApacheKafkaConfiguration>();

            Logger = new LoggerConfiguration().WriteTo.Kafka(bootstrapServers: logConfig.BootstrapServer,
                topic: logConfig.Topics[index]).CreateLogger();
        }
    }
}
