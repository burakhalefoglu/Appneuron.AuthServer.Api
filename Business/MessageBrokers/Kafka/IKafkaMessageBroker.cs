using Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.MessageBrokers.Kafka
{
    public interface IKafkaMessageBroker
    {
        Task<IResult> SendMessageAsync<T>(T messageModel) where T :
         class, new();

        Task GetProjectCreationMessage();

    }
}
