namespace Core.Entities.ClaimModels;

public class ClientClaimModel
{
    public long ClientId { get; set; }
    public long ProjectId { get; set; }
    public string[] OperationClaims { get; set; }
}