using Business.Handlers.UserProjects.Commands;
using FluentValidation;

namespace Business.Handlers.UserProjects.ValidationRules
{
    public class CreateUserProjectValidator : AbstractValidator<CreateUserProjectCommand>
    {
        public CreateUserProjectValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProjectKey).NotEmpty();
        }
    }

    public class UpdateUserProjectValidator : AbstractValidator<UpdateUserProjectCommand>
    {
        public UpdateUserProjectValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProjectKey).NotEmpty();
        }
    }
}