using System.Collections.Generic;

namespace Core.Entities.Concrete
{
    public class UserClaim : IEntity
    {
        public int UsersId { get; set; }
        public int ClaimId { get; set; }
        public long Id { get; set; }

        public virtual OperationClaim Claim { get; set; }
        public virtual User Users { get; set; }

    }
}