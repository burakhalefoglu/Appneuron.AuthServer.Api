namespace Core.Entities.Concrete
{
    public class GroupClaim : IEntity
    {
        public int GroupId { get; set; }
        public int ClaimId { get; set; }

        public virtual OperationClaim OperationClaim { get; set; }
        public virtual Group Group { get; set; }
    }
}