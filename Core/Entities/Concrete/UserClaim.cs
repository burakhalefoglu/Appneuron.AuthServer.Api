namespace Core.Entities.Concrete
{
    public class UserClaim : DocumentDbEntity
    {
        public bool Status = true;
        public string UsersId { get; set; }
        public string ClaimId { get; set; }
    }
}