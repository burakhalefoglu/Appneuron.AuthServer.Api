﻿using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Handlers.UserGroups.Queries;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Internals.Handlers.UserGroups.Queries
{
    public class GetUserGroupInternalQuery : IRequest<IDataResult<UserGroup>>
    {
        public string UserId { get; set; }

        public class GetUserGroupInternalQueryHandler : IRequestHandler<GetUserGroupInternalQuery, IDataResult<UserGroup>>
        {
            private readonly IUserGroupRepository _userGroupRepository;

            public GetUserGroupInternalQueryHandler(IUserGroupRepository userGroupRepository)
            {
                _userGroupRepository = userGroupRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheAspect(10)]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IDataResult<UserGroup>> Handle(GetUserGroupInternalQuery request,
                CancellationToken cancellationToken)
            {
                var userGroup = await _userGroupRepository.GetAsync(p => p.UserId == request.UserId);
                return new SuccessDataResult<UserGroup>(userGroup);
            }
        }
    }
}