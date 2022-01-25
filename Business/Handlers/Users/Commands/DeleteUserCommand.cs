using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Transaction;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.Users.Commands
{
    public class DeleteUserCommand : IRequest<IResult>
    {
        public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IResult>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IUserRepository _userRepository;

            public DeleteUserCommandHandler(IUserRepository userRepository,
                IHttpContextAccessor httpContextAccessor)
            {
                _userRepository = userRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(LogstashLogger))]
            [TransactionScopeAspectAsync]
            public async Task<IResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
            {
                var userId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var userToDelete = await _userRepository.GetAsync(p => p.UserId == userId);
                if (userToDelete == null) return new ErrorResult(Messages.UserNotFound);
                userToDelete.Status = false;

                await _userRepository.UpdateAsync(userToDelete, x=> x.UserId == userToDelete.UserId);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}