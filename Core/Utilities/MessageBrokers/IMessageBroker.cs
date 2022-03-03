using Core.Utilities.Results;

namespace Core.Utilities.MessageBrokers
{
    public interface IMessageBroker
    {
        Task<IResult> SendMessageAsync<T>(T messageModel) where T :
            class, new();
    }
}