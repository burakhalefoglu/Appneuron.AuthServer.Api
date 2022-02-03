using Serilog;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Loggers
{
    public class ConsoleLogger : LoggerServiceBase
    {
        public ConsoleLogger()
        {

            _ = new LoggerConfiguration()
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}
