using System.Threading.Tasks;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;

namespace Business.Abstract
{
    public interface IAuthService
    {
        Task<IResult> Register(User user);
    }
}