using Business.Handlers.Authorizations.Commands;
using FluentValidation;

namespace Business.Handlers.Authorizations.ValidationRules;

public class RegisterUserValidator : AbstractValidator<LoginOrRegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(p => p.Password).Password();
    }
}