using FluentValidation;

namespace Application.Commands.User
{
    public class CreateUserInformationCommandValidator : AbstractValidator<CreateUserInformationCommand>
    {
        public CreateUserInformationCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email is not valid!");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is required");
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("PhoneNumber is required");
            RuleFor(x => x.Nationality).NotEmpty().WithMessage("Nationality is required");
            RuleFor(x => x.Status).IsInEnum().WithMessage("Status is required");
            RuleFor(x => x.Role).IsInEnum().WithMessage("Role is required");
        }
    }
}