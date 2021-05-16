using System.Collections.Generic;

namespace Core.Entities.Concrete
{
    public class UserGroup : IEntity
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
        public virtual Group Group { get; set; }

    }
}