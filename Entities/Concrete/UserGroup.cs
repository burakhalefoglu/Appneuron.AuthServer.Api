using Core.Entities;

namespace Entities.Concrete;

public class UserGroup : IEntity
{
    public UserGroup()
    {
        CreatedAt = DateTimeOffset.Now;
        Status = true;
    }

    public DateTimeOffset CreatedAt { get; set; }
    public long GroupId { get; set; }
    public long UserId { get; set; }
    public bool Status { get; set; }
    public long Id { get; set; }
}