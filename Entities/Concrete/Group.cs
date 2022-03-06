using System.Globalization;
using Core.Entities;

namespace Entities.Concrete
{
    public class Group : IEntity
    {
        public Group()
        {
            CreatedAt = DateTimeOffset.Now;
            Status = true;
        }

        public DateTimeOffset CreatedAt { get; set; }
        public bool Status { get; set; }
        public string GroupName { get; set; }
        public long Id { get; set; }
    }
}