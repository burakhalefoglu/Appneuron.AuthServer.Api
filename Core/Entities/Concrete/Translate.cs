namespace Core.Entities.Concrete
{
    public class Translate : IEntity
    {
        public bool Status = true;
        public string Code { get; set; }
        public string Value { get; set; }
        public long Id { get; set; }
    }
}