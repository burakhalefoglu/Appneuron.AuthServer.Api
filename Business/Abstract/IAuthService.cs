using Core.Entities.ApiModel;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;

namespace Business.Abstract;

public interface IAuthService
{
     Task<IDataResult<AccessToken>> Register(Register client);
}