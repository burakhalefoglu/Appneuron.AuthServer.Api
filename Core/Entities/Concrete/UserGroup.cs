namespace Core.Entities.Concrete
{
    public class UserGroup : DocumentDbEntity
    {
        public bool Status = true;
        public string GroupId { get; set; }
        public string UserId { get; set; }
    }
}