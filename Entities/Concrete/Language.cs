using Core.Entities;

namespace Entities.Concrete;

public class Language : IEntity
{
    public Language()
    {
        CreatedAt = DateTimeOffset.Now;
        Status = true;
    }

    public DateTimeOffset CreatedAt { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public bool Status { get; set; }
    public long Id { get; set; }
}