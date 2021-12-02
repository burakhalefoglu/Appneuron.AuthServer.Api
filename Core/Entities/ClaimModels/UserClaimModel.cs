namespace Core.Entities.ClaimModels
{
    public class UserClaimModel
    {
        public int UserId { get; set; }
        public string[] OperationClaims { get; set; }
    }
}