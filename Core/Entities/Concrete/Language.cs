namespace Core.Entities.Concrete
{
    public class Language : DocumentDbEntity
    {
        public bool Status = true;
        public string Name { get; set; }
        public string Code { get; set; }
    }
}