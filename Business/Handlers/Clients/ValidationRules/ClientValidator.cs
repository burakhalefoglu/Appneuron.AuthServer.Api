
using Business.Handlers.Clients.Commands;
using FluentValidation;

namespace Business.Handlers.Clients.ValidationRules
{

    public class CreateClientValidator : AbstractValidator<CreateClientCommand>
    {
        public CreateClientValidator()
        {
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.CustomerId).NotEmpty();

        }
    }
    public class UpdateClientValidator : AbstractValidator<UpdateClientCommand>
    {
        public UpdateClientValidator()
        {
            RuleFor(x => x.ClientId).NotEmpty();
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.CustomerId).NotEmpty();

        }
    }
}