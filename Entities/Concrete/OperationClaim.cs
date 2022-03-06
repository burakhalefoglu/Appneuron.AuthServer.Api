using System.Globalization;
using Core.Entities;

namespace Entities.Concrete
{
    public class OperationClaim : IEntity
    {
        public OperationClaim()
        {
            CreatedAt = DateTimeOffset.Now;
            Status = true;
        }

        public DateTimeOffset CreatedAt { get; set; }
        public bool Status { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }
        public long Id { get; set; }
    }
}