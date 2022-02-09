namespace Core.Entities.ClaimModels
{
    public class UserClaimModel
    {
        public long UserId { get; set; }
        public string[] OperationClaims { get; set; }
    }
}