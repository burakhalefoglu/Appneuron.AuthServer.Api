namespace Business.MessageBrokers.Models;

public class ProjectMessageCommand
{
    public long UserId { get; set; }
    public long ProjectId { get; set; }
    public string Email { get; set; }
}