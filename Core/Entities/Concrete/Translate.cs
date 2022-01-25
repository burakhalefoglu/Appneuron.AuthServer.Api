namespace Core.Entities.Concrete
{
    public class Translate : DocumentDbEntity
    {
        public int LangId { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }

        public bool Status = true;
    }
}