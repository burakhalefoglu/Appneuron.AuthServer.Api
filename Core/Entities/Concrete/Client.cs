namespace Core.Entities.Concrete
{
    public class Client : IEntity
    {
        public bool Status = true;
        public long ProjectId { get; set; }
        public long Id { get; set; }
    }
}