using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.FastConsole;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Loggers
{
    public class ConsoleLogger : LoggerServiceBase
    {
        public ConsoleLogger()
        {
            var config = new FastConsoleSinkOptions { UseJson = true };
            _ = new LoggerConfiguration()
                .WriteTo.FastConsole(
                    config,
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}
