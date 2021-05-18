using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.Concrete
{
    public class ClientClaim : IEntity
    {
        public long ClientId { get; set; }
        public int ClaimId { get; set; }

        public virtual OperationClaim Claim { get; set; }
        public virtual Client Clients { get; set; }
    }
}
