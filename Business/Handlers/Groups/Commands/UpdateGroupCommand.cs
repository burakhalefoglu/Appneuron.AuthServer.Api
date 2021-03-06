using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Groups.Commands;

public class UpdateGroupCommand : IRequest<IResult>
{
    public string GroupName { get; set; }

    public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, IResult>
    {
        private readonly IGroupRepository _groupRepository;

        public UpdateGroupCommandHandler(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        [SecuredOperation(Priority = 1)]
        [CacheRemoveAspect("Get")]
        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IResult> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            var isGroupExist =
                await _groupRepository.GetAsync(g => g.GroupName == request.GroupName && g.Status == true);

            if (isGroupExist == null) return new ErrorResult(Messages.GroupNotFound);

            var groupToUpdate = new Group
            {
                GroupName = request.GroupName
            };
            await _groupRepository.UpdateAsync(groupToUpdate);
            return new SuccessResult(Messages.Updated);
        }
    }
}