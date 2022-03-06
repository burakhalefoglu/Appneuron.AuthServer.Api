using Core.Entities;

namespace Entities.Concrete
{
    public class UserClaim : IEntity
    {
        public UserClaim()
        {
            CreatedAt = DateTime.Now;
            Status = true;
        }

        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }
        public long UsersId { get; set; }
        public long ClaimId { get; set; }
        public long Id { get; set; }
    }
}