using Core.Entities;

namespace Entities.Concrete
{
    public class OperationClaim : IEntity
    {
        public OperationClaim()
        {
            CreatedAt = DateTime.Now;
            Status = true;
        }

        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }
        public long Id { get; set; }
    }
}