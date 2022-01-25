namespace Core.Entities.Concrete
{
    public class GroupClaim : DocumentDbEntity
    {
        public int GroupId { get; set; }
        public int ClaimId { get; set; }

        public bool Status = true;
    }
}