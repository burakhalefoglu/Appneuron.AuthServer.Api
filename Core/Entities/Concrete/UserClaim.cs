namespace Core.Entities.Concrete
{
    public class UserClaim : IEntity
    {
        public bool Status = true;
        public long UsersId { get; set; }
        public long ClaimId { get; set; }
        public long Id { get; set; }
    }
}