namespace Core.Entities.ClaimModels
{
    public class ClientClaimModel
    {
        public string ClientId { get; set; }
        public int CustomerId { get; set; }
        public string ProjectId { get; set; }
        public string[] OperationClaims { get; set; }
    }
}