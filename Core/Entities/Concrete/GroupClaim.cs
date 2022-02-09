namespace Core.Entities.Concrete
{
    public class GroupClaim : IEntity
    {
        public bool Status = true;
        public long GroupId { get; set; }
        public long ClaimId { get; set; }
        public long Id { get; set; }
    }
}