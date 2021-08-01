namespace Business.MessageBrokers.Models
{
    public class ProjectMessageCommand
    {
        public int UserId { get; set; }
        public string ProjectKey { get; set; }
    }
}