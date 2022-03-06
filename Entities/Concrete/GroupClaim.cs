using Core.Entities;

namespace Entities.Concrete
{
    public class GroupClaim : IEntity
    {
        public GroupClaim()
        {
            CreatedAt = DateTime.Now;
            Status = true;
        }

        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }
        public long GroupId { get; set; }
        public long ClaimId { get; set; }
        public long Id { get; set; }
    }
}