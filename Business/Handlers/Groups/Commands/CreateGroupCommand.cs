using System.Threading;
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

namespace Business.Handlers.Groups.Commands
{
    public class CreateGroupCommand : IRequest<IResult>
    {
        public string GroupName { get; set; }

        public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, IResult>
        {
            private readonly IGroupRepository _groupRepository;

            public CreateGroupCommandHandler(IGroupRepository groupRepository)
            {
                _groupRepository = groupRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(LogstashLogger))]
            public async Task<IResult> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
            {
                var groupExist = await _groupRepository.GetAsync(g => g.GroupName == request.GroupName);
                if (groupExist != null)
                {
                    return new ErrorResult(Messages.NameAlreadyExist);
                }
                var group = new Group
                {
                    GroupName = request.GroupName
                };
                await _groupRepository.AddAsync(@group);
                return new SuccessResult(Messages.Added);
            }
        }
    }
}