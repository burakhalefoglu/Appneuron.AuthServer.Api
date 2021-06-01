using AutoMapper;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Core.Aspects.Autofac.Transaction;
using System.Collections.Generic;
using Core.Entities.Concrete;
using Business.Fakes.Handlers.UserClaims;

namespace Business.Handlers.Users.Commands
{
    public class DeleteUserCommand : IRequest<IResult>
    {
        public class DeleteAnimalCommandHandler : IRequestHandler<DeleteUserCommand, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMediator _mediator;

            public DeleteAnimalCommandHandler(IUserRepository userRepository,
                IMapper mapper,
               IMediator mediator)
            {
                _userRepository = userRepository;
                _mapper = mapper;
                _mediator = mediator;
                _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [TransactionScopeAspectAsync]
            public async Task<IResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
            {
                var UserId = int.Parse(_httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var userToDelete = _userRepository.Get(p => p.UserId == UserId);
                userToDelete.Status = false;

                //var result = await _mediator.Send(new GetUserClaimLookupInternalQuery()
                //{
                //    UserId = UserId
                //});
                //userToDelete.UserClaims = result.Data.ToList();
                _userRepository.Delete(userToDelete);
                await _userRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}