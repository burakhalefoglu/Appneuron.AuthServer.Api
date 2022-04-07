namespace Core.Entities.ClaimModels;

public class UserClaimModel
{
    public long UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string[] OperationClaims { get; set; }
}