namespace Core.Entities.Concrete
{
    public class Language : IEntity
    {
        public bool Status = true;
        public string Name { get; set; }
        public string Code { get; set; }
        public long Id { get; set; }
    }
}