namespace Core.Entities.Concrete
{
    public class UserGroup : IEntity
    {
        public bool Status = true;
        public long GroupId { get; set; }
        public long UsersId { get; set; }
        public long Id { get; set; }
    }
}