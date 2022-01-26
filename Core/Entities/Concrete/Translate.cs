namespace Core.Entities.Concrete
{
    public class Translate : DocumentDbEntity
    {
        public bool Status = true;
        public string Code { get; set; }
        public string Value { get; set; }
    }
}