using System;
using System.Collections.Generic;
using System.Text;

namespace Business.MessageBrokers.SignalR.Models
{
    public class CustomerModel
    {
        public int UserId { get; set; }
        public string ConnectionId { get; set; }
    }
}
