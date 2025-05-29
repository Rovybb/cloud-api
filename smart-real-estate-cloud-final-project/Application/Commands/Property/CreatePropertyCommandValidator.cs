using FluentValidation;

namespace Application.Commands.Property
{
    public class CreatePropertyCommandValidator : AbstractValidator<CreatePropertyCommand>
    {
        public CreatePropertyCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title cannot be empty!");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be empty!");
            //RuleFor(x => x.Status).IsInEnum().WithMessage("Status is not valid!");
            //RuleFor(x => x.Type).IsInEnum().WithMessage("Type is not valid!");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0!");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address cannot be empty!");
            RuleFor(x => x.Area).NotEmpty().WithMessage("Area cannot be empty!");
            RuleFor(x => x.Rooms).GreaterThanOrEqualTo(0).WithMessage("Rooms must be greater than or equal to 0!");
            RuleFor(x => x.Bathrooms).GreaterThanOrEqualTo(0).WithMessage("Bathrooms must be greater than or equal to 0!");
            RuleFor(x => x.ConstructionYear).GreaterThan(1500).WithMessage("Construction year cannot be empty!");
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
