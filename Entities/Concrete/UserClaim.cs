using Core.Entities;

namespace Entities.Concrete;

public class UserClaim : IEntity
{
    public UserClaim()
    {
        CreatedAt = DateTimeOffset.Now;
        Status = true;
    }

    public DateTimeOffset CreatedAt { get; set; }
    public long UsersId { get; set; }
    public long ClaimId { get; set; }
    public bool Status { get; set; }
    public long Id { get; set; }
}