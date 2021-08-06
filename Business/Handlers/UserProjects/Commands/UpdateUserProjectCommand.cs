using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.UserProjects.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.UserProjects.Commands
{
    public class UpdateUserProjectCommand : IRequest<IResult>
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string ProjectKey { get; set; }

        public class UpdateUserProjectCommandHandler : IRequestHandler<UpdateUserProjectCommand, IResult>
        {
            private readonly IUserProjectRepository _userProjectRepository;
            private readonly IMediator _mediator;

            public UpdateUserProjectCommandHandler(IUserProjectRepository userProjectRepository, IMediator mediator)
            {
                _userProjectRepository = userProjectRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateUserProjectValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ApacheKafkaDatabaseActionLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateUserProjectCommand request, CancellationToken cancellationToken)
            {
                var isThereUserProjectRecord = await _userProjectRepository.GetAsync(u => u.Id == request.Id);

                isThereUserProjectRecord.UserId = request.UserId;
                isThereUserProjectRecord.ProjectKey = request.ProjectKey;

                _userProjectRepository.Update(isThereUserProjectRecord);
                await _userProjectRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}