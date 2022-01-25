namespace Core.Entities.Concrete
{
    public class Client : DocumentDbEntity
    {
        public string ClientId { get; set; }
        public string ProjectId { get; set; }

        public bool Status = true;
    }
}