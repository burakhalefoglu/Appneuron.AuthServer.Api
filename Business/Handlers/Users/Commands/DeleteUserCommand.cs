﻿using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Transaction;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;
using IResult = Core.Utilities.Results.IResult;

namespace Business.Handlers.Users.Commands;

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
        [LogAspect(typeof(ConsoleLogger))]
        [TransactionScopeAspect]
        public async Task<IResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var email = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type.EndsWith("emailaddress"))?.Value;

            var userToDelete = await _userRepository.GetAsync(p => p.Email == email && p.Status == true);
            if (userToDelete == null) return new ErrorResult(Messages.UserNotFound);
            await _userRepository.DeleteAsync(userToDelete);
            return new SuccessResult(Messages.Deleted);
        }
    }
}