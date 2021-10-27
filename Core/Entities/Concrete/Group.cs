using System.Collections.Generic;

namespace Core.Entities.Concrete
{
    public class Group : IEntity
    {
        public int Id { get; set; }
        public string GroupName { get; set; }

        public virtual ICollection<GroupClaim> GroupClaims { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
    }
}