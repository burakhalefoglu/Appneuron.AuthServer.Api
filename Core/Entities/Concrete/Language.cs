namespace Core.Entities.Concrete
{
    public class Language : IEntity
    {
        public Language()
        {
            CreatedAt = DateTime.Now;
            Status = true;
        }

        public DateTime CreatedAt { get; }
        public bool Status { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public long Id { get; set; }
    }
}