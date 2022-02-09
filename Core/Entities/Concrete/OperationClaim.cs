namespace Core.Entities.Concrete
{
    public class OperationClaim : IEntity
    {
        public bool Status = true;
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }
        public long Id { get; set; }
    }
}