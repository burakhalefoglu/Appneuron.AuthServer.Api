using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.UserProjects.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.UserProjects.Commands
{
    public class UpdateUserProjectCommand : IRequest<IResult>
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ProjectId { get; set; }

        public class UpdateUserProjectCommandHandler : IRequestHandler<UpdateUserProjectCommand, IResult>
        {
            private readonly IUserProjectRepository _userProjectRepository;

            public UpdateUserProjectCommandHandler(IUserProjectRepository userProjectRepository)
            {
                _userProjectRepository = userProjectRepository;
            }

            [ValidationAspect(typeof(UpdateUserProjectValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateUserProjectCommand request, CancellationToken cancellationToken)
            {
                var isThereUserProjectRecord = await _userProjectRepository.GetAsync(u => u.Id == request.Id && u.Status == true);

                if (isThereUserProjectRecord == null) return new ErrorResult(Messages.UserProjectNotFound);

                isThereUserProjectRecord.UserId = request.UserId;
                isThereUserProjectRecord.ProjectId = request.ProjectId;

                await _userProjectRepository.UpdateAsync(isThereUserProjectRecord);
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}