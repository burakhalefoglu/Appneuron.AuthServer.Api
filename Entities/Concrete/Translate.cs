using Core.Entities;

namespace Entities.Concrete;

public class Translate : IEntity
{
    public Translate()
    {
        CreatedAt = DateTimeOffset.Now;
        Status = true;
    }

    public DateTimeOffset CreatedAt { get; set; }
    public string Code { get; set; }
    public string Value { get; set; }
    public bool Status { get; set; }
    public long Id { get; set; }
}