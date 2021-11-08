﻿using System.Threading;
using System.Threading.Tasks;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Fakes.Handlers.UserProjects
{
    public class GetUserProjectInternalQuery : IRequest<IDataResult<UserProject>>
    {
        public string ProjectKey { get; set; }

        public class GetUserProjectQueryHandler : IRequestHandler<GetUserProjectInternalQuery, IDataResult<UserProject>>
        {
            private readonly IMediator _mediator;
            private readonly IUserProjectRepository _userProjectRepository;

            public GetUserProjectQueryHandler(IUserProjectRepository userProjectRepository, IMediator mediator)
            {
                _userProjectRepository = userProjectRepository;
                _mediator = mediator;
            }

            public async Task<IDataResult<UserProject>> Handle(GetUserProjectInternalQuery request,
                CancellationToken cancellationToken)
            {
                var userProject = await _userProjectRepository.GetAsync(p => p.ProjectKey == request.ProjectKey);
                return new SuccessDataResult<UserProject>(userProject);
            }
        }
    }
}