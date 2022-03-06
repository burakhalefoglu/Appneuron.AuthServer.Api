using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class UserProject : IEntity
    {
        public UserProject()
        {
            CreatedAt = DateTimeOffset.Now;
            Status = true;
        }
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ProjectId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public bool Status { get; set; }

    }
}