using Core.Entities;

namespace Entities.Concrete;

public class RefreshToken: IEntity
{
    public RefreshToken()
    {
        Status = true;
        CreatedAt = DateTimeOffset.Now;
    }
    public long Id { get; set; }

    public long UserId { get; set; }
    public bool Status { get; set; }
    
    public string Value { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}