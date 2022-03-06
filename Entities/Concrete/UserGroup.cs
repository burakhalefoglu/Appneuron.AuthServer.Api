using Core.Entities;

namespace Entities.Concrete
{
    public class UserGroup : IEntity
    {
        public UserGroup()
        {
            CreatedAt = DateTime.Now;
            Status = true;
        }

        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }
        public long GroupId { get; set; }
        public long UsersId { get; set; }
        public long Id { get; set; }
    }
}