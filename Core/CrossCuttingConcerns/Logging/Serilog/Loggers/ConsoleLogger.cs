using Serilog;
using Serilog.Sinks.FastConsole;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Loggers
{
    public class ConsoleLogger : LoggerServiceBase
    {
        public ConsoleLogger()
        {

            _ = new LoggerConfiguration()
                .WriteTo.FastConsole(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}
