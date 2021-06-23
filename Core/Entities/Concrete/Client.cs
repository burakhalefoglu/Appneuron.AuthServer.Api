using System.Collections.Generic;

namespace Core.Entities.Concrete
{
    public class Client : IEntity
    {
        public long Id { get; set; }
        public string ClientId { get; set; }
        public string ProjectId { get; set; }
        public int CustomerId { get; set; }
        public virtual User user { get; set; }
        public virtual ICollection<ClientGroup> ClientGroups { get; set; }
        public virtual ICollection<ClientClaim> ClientClaims { get; set; }
    }
}