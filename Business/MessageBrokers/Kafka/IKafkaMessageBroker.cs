using System.Threading.Tasks;
using Core.Utilities.Results;

namespace Business.MessageBrokers.Kafka
{
    public interface IKafkaMessageBroker
    {
        Task<IResult> SendMessageAsync<T>(T messageModel) where T :
            class, new();

        Task GetProjectCreationMessage();
    }
}