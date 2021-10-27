using System.Collections.Generic;

namespace Core.Entities.Concrete
{
    public class Client : IEntity
    {
        public long Id { get; set; }
        public string ClientId { get; set; }
        public string ProjectId { get; set; }
    }
}