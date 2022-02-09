using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Internals.Handlers.UserProjects
{
    public class CreateUserProjectInternalCommand : IRequest<IResult>
    {
        public long UserId { get; set; }
        public long ProjectId { get; set; }

        public class
            CreateUserProjectInternalCommandHandler : IRequestHandler<CreateUserProjectInternalCommand, IResult>
        {
            private readonly IUserProjectRepository _userProjectRepository;

            public CreateUserProjectInternalCommandHandler(IUserProjectRepository userProjectRepository)
            {
                _userProjectRepository = userProjectRepository;
            }
            
            public async Task<IResult> Handle(CreateUserProjectInternalCommand request,
                CancellationToken cancellationToken)
            {
                var isThereUserProjectRecord =
                    await _userProjectRepository.AnyAsync(u => u.ProjectId == request.ProjectId && u.Status == true);

                if (isThereUserProjectRecord)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedUserProject = new UserProject
                {
                    UserId = request.UserId,
                    ProjectId = request.ProjectId
                };

                await _userProjectRepository.AddAsync(addedUserProject);
                return new SuccessResult(Messages.Added);
            }
        }
    }
}