namespace Core.Entities.Concrete
{
    public class Client : DocumentDbEntity
    {
        public bool Status = true;
        public string ProjectId { get; set; }
    }
}