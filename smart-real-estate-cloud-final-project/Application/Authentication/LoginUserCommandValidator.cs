using FluentValidation;

namespace Application.Commands.User
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email is not valid!");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).WithMessage("Password is required and must be at least 8 characters long");
        }
    }
}