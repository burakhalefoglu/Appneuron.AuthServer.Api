using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Handlers.Users.Commands;

public class CreateUserCommand : IRequest<IResult>
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string MobilePhones { get; set; }
    public bool Status { get; set; }
    public DateTime RecordDate { get; set; }
    public DateTime UpdateContactDate { get; set; }
    public string Password { get; set; }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IResult>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [SecuredOperation(Priority = 1)]
        [CacheRemoveAspect("Get")]
        [LogAspect(typeof(ConsoleLogger))]
        public async Task<IResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var userExits = await _userRepository.GetAsync(u => u.Email == request.Email && u.Status == true);

            if (userExits != null)
                return new ErrorResult(Messages.NameAlreadyExist);

            var user = new User
            {
                Email = request.Email,
                Name = request.Name,
                Status = true
            };

            await _userRepository.AddAsync(user);
            return new SuccessResult(Messages.Added);
        }
    }
}