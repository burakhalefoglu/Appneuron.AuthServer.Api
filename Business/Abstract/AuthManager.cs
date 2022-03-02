using System.Threading.Tasks;
using Business.Constants;
using Business.Handlers.Authorizations.ValidationRules;
using Business.MessageBrokers;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;

namespace Business.Abstract
{
    public class AuthManager: IAuthService
    {

        public AuthManager()
        {
        }

        [PerformanceAspect(5)]
        [ValidationAspect(typeof(RegisterUserValidator), Priority = 2)]
        [CacheRemoveAspect("Get")]
        [LogAspect(typeof(ConsoleLogger))]
        [TransactionScopeAspect]
        public async Task<IResult> Register(User user)
        {
            return new SuccessResult(user.Email);

        }
    }
}