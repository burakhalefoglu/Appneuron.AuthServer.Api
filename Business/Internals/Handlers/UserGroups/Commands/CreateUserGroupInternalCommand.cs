﻿using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Internals.Handlers.UserGroups.Commands
{
    public class CreateUserGroupInternalCommand : IRequest<IResult>
    {
        public string GroupId { get; set; }
        public string UserId { get; set; }

        public class CreateUserGroupInternalCommandHandler : IRequestHandler<CreateUserGroupInternalCommand, IResult>
        {
            private readonly IUserGroupRepository _userGroupRepository;

            public CreateUserGroupInternalCommandHandler(IUserGroupRepository userGroupRepository)
            {
                _userGroupRepository = userGroupRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IResult> Handle(CreateUserGroupInternalCommand request, CancellationToken cancellationToken)
            {
                var userGroup = new UserGroup
                {
                    GroupId = request.GroupId,
                    UserId = request.UserId
                };

                await _userGroupRepository.AddAsync(userGroup);
                return new SuccessResult(Messages.Added);
            }
        }
    }
}