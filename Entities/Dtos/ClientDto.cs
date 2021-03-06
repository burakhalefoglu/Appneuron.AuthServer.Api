using Core.Entities;

namespace Entities.Dtos;

public class ClientDto : IDto
{
    public string ClientId { get; set; }
    public int CustomerId { get; set; }
    public string ProjectId { get; set; }
}