using System;

namespace Core.Entities.Concrete
{
    public class Log : DocumentDbEntity
    {
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string Exception { get; set; }
    }
}