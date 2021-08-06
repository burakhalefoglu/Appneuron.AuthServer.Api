using System;
using System.Collections.Generic;
using System.Text;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka
{
    public class ApacheKafkaForgotResetLogger : BaseKafkaLogger
    {
        public ApacheKafkaForgotResetLogger() : base(2)
        {

        }
    }
}
