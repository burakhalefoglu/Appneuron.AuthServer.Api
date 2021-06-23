using Business.Handlers.ClientGroups.Commands;
using FluentValidation;

namespace Business.Handlers.ClientGroups.ValidationRules
{
    public class CreateClientGroupValidator : AbstractValidator<CreateClientGroupCommand>
    {
        public CreateClientGroupValidator()
        {
            RuleFor(x => x.ClientId).NotEmpty();
        }
    }

    public class UpdateClientGroupValidator : AbstractValidator<UpdateClientGroupCommand>
    {
        public UpdateClientGroupValidator()
        {
            RuleFor(x => x.ClientId).NotEmpty();
        }
    }
}