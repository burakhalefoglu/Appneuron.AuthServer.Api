namespace Core.Entities.Concrete
{
    public class Group : DocumentDbEntity
    {
        public bool Status = true;
        public string GroupName { get; set; }
    }
}