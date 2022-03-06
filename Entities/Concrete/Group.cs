using Core.Entities;

namespace Entities.Concrete
{
    public class Group : IEntity
    {
        public Group()
        {
            CreatedAt = DateTime.Now;
            Status = true;
        }

        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }
        public string GroupName { get; set; }
        public long Id { get; set; }
    }
}