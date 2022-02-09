using System;
using Core.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Core.Utilities.IoC;
using Core.Utilities.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.Http.BatchFormatters;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Loggers
{
    public class LogstashLogger : LoggerServiceBase
    {
        public LogstashLogger()
        {
            var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();

            var logConfig = configuration.GetSection("SeriLogConfigurations:LogstashConfiguration")
                .Get<LogstashConfiguration>() ?? throw new Exception(SerilogMessages.NullOptionsMessage);

            var seriLogConfig = new LoggerConfiguration()
                .WriteTo
                .DurableHttpUsingFileSizeRolledBuffers(
                    $"http://{logConfig.Host}:{logConfig.Port}",
                    batchFormatter: new ArrayBatchFormatter()
                )
                .CreateLogger();
            Logger = seriLogConfig;
        }
    }
}
