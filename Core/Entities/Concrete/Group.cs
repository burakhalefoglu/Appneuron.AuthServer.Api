namespace Core.Entities.Concrete
{
    public class Group : IEntity
    {
        public bool Status = true;
        public string GroupName { get; set; }
        public long Id { get; set; }
    }
}