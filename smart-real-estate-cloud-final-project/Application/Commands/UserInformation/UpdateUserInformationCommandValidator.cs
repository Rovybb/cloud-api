using FluentValidation;

namespace Application.Commands.User
{
    public class UpdateUserInformationCommandValidator : AbstractValidator<UpdateUserInformationCommand>
    {
        public UpdateUserInformationCommandValidator()
        {
            RuleFor(x => x.Request.Username).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.Request.Email).NotEmpty().EmailAddress().WithMessage("Email is not valid!");
            RuleFor(x => x.Request.FirstName).NotEmpty().WithMessage("FirstName is required");
            RuleFor(x => x.Request.LastName).NotEmpty().WithMessage("LastName is required");
            RuleFor(x => x.Request.Status).IsInEnum().WithMessage("Status is required");
            RuleFor(x => x.Request.Address).NotEmpty().WithMessage("Address is required");
            RuleFor(x => x.Request.PhoneNumber).NotEmpty().WithMessage("PhoneNumber is required");
            RuleFor(x => x.Request.Nationality).NotEmpty().WithMessage("Nationality is required");
            RuleFor(x => x.Request.Role).IsInEnum().WithMessage("Role is required");
        }
    }
}
