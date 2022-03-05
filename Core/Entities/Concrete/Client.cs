namespace Core.Entities.Concrete
{
    public class Client : IEntity
    {

        public Client()
        {
            CreatedAt = DateTime.Now;
            Status = true;
        }
        public bool Status  { get; set; }
        public DateTime CreatedAt { get; }
        public long ProjectId { get; set; }
        public long Id { get; set; }
    }
}