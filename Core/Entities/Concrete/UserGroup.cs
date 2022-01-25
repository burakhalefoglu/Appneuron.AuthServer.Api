namespace Core.Entities.Concrete
{
    public class UserGroup : DocumentDbEntity
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        
        public bool Status = true;
    }
}