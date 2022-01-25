namespace Core.Entities.Concrete
{
    public class UserClaim : DocumentDbEntity
    {
        public int UsersId { get; set; }
        public int ClaimId { get; set; }
        public bool Status = true;
    }
}