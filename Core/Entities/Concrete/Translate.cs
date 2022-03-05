namespace Core.Entities.Concrete
{
    public class Translate : IEntity
    {
        public Translate()
        {
            CreatedAt = DateTime.Now;
            Status = true;
        }

        public DateTime CreatedAt { get; }
        public bool Status { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public long Id { get; set; }
    }
}