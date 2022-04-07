namespace Core.Entities.Concrete;

public class Log : IEntity
{
    public Log()
    {
        TimeStamp = DateTime.Now;
        Status = true;
    }

    public string MessageTemplate { get; set; }
    public string Level { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    public string Exception { get; set; }
    public long Id { get; set; }
    public bool Status { get; set; }
}