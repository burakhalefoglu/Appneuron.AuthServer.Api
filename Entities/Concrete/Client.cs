using System.Globalization;
using Core.Entities;

namespace Entities.Concrete
{
    public class Client : IEntity
    {

        public Client()
        {
            CreatedAt = DateTimeOffset.Now;
            Status = true;
        }
        public bool Status  { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public long ProjectId { get; set; }
        public long Id { get; set; }
    }
}