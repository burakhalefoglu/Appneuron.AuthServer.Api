namespace Core.Entities.Concrete
{
    public class GroupClaim : DocumentDbEntity
    {
        public bool Status = true;
        public string GroupId { get; set; }
        public string ClaimId { get; set; }
    }
}