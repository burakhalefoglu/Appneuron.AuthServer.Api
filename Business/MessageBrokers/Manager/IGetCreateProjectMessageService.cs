using System.Threading.Tasks;
using Business.MessageBrokers.Models;
using Core.Utilities.Results;

namespace Business.MessageBrokers.Manager
{
    public interface IGetCreateProjectMessageService
    {
        Task<IResult> GetProjectCreationMessageQuery(ProjectMessageCommand message);
    }
}