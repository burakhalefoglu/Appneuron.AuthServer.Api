
using Business.Handlers.Clients.Commands;
using FluentValidation;

namespace Business.Handlers.Clients.ValidationRules
{

    public class CreateTokenValidator : AbstractValidator<CreateTokenCommand>
    {
        public CreateTokenValidator()
        {
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.ProjectId).NotEmpty();

        }
    }

}