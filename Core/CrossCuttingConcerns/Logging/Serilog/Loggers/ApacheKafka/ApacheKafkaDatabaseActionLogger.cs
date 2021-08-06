using System;
using System.Collections.Generic;
using System.Text;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka
{
    public class ApacheKafkaDatabaseActionLogger : BaseKafkaLogger
    {
        public ApacheKafkaDatabaseActionLogger() : base(3)
        {

        }
    }
}
