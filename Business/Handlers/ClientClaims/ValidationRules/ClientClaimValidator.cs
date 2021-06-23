using Business.Handlers.ClientClaims.Commands;
using FluentValidation;

namespace Business.Handlers.ClientClaims.ValidationRules
{
    public class CreateClientClaimValidator : AbstractValidator<CreateClientClaimCommand>
    {
        public CreateClientClaimValidator()
        {
            RuleFor(x => x.ClaimId).NotEmpty();
        }
    }

    public class UpdateClientClaimValidator : AbstractValidator<UpdateClientClaimCommand>
    {
        public UpdateClientClaimValidator()
        {
            RuleFor(x => x.ClaimId).NotEmpty();
        }
    }
}