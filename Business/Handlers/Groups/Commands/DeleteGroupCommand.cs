using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Groups.Commands
{
    public class DeleteGroupCommand : IRequest<IResult>
    {
        public string Id { get; set; }

        public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, IResult>
        {
            private readonly IGroupRepository _groupRepository;

            public DeleteGroupCommandHandler(IGroupRepository groupRepository)
            {
                _groupRepository = groupRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IResult> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
            {
                var groupToDelete = await _groupRepository
                    .GetAsync(x => x.ObjectId == request.Id);

                if (groupToDelete == null) return new ErrorResult(Messages.GroupNotFound);
                groupToDelete.Status = false;
                await _groupRepository.UpdateAsync(groupToDelete, x => x.ObjectId == groupToDelete.ObjectId);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}