using System.Globalization;
using Core.Entities;

namespace Entities.Concrete
{
    public class GroupClaim : IEntity
    {
        public GroupClaim()
        {
            CreatedAt = DateTimeOffset.Now;
            Status = true;
        }

        public DateTimeOffset CreatedAt { get; set; }
        public bool Status { get; set; }
        public long GroupId { get; set; }
        public long ClaimId { get; set; }
        public long Id { get; set; }
    }
}