using System.Collections.Generic;

namespace Core.Entities.Concrete
{
    public class OperationClaim : DocumentDbEntity
    {
        public int ClaimId { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }

        public bool Status = true;
    }
}