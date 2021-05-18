using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.Concrete
{
    public class ClientGroup : IEntity
    {
        public int GroupId { get; set; }
        public long ClientId { get; set; }

        public virtual Client client { get; set; }
        public virtual Group Group { get; set; }
    }
}
