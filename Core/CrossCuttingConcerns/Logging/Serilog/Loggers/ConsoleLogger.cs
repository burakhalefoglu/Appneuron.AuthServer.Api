using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Loggers
{
    class ConsoleLogger
    {
        public ConsoleLogger()
        {

            _ = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}
