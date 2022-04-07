namespace Business.MessageBrokers.Models;

public class CreateCustomerMessageCommand
{
    public long DemographicId { get; set; }
    public long IndustryId { get; set; }
    public long Id { get; set; }
}