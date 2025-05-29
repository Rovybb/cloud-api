using FluentValidation;

namespace Application.Commands.Property
{
    public class UpdatePropertyCommandValidator : AbstractValidator<UpdatePropertyCommand>
    {
        public UpdatePropertyCommandValidator()
        {
            RuleFor(x => x.Request.Title).NotEmpty().WithMessage("Title cannot be empty!");
            RuleFor(x => x.Request.Description).NotEmpty().WithMessage("Description cannot be empty!");
            //RuleFor(x => x.Request.Type).IsInEnum().WithMessage("Type is not valid!");
            //RuleFor(x => x.Request.Status).IsInEnum().WithMessage("Status is not valid!");
            RuleFor(x => x.Request.Price).GreaterThan(0).WithMessage("Price must be greater than 0!");
            RuleFor(x => x.Request.Area).NotEmpty().WithMessage("Area cannot be empty!");
            RuleFor(x => x.Request.Rooms).GreaterThanOrEqualTo(0).WithMessage("Rooms must be greater than or equal to 0!");
            RuleFor(x => x.Request.Address).NotEmpty().WithMessage("Address cannot be empty!");
            RuleFor(x => x.Request.Bathrooms).GreaterThanOrEqualTo(0).WithMessage("Bathrooms must be greater than or equal to 0!");
            RuleFor(x => x.Request.ConstructionYear).GreaterThan(1500).WithMessage("Construction year cannot be empty!");
            RuleFor(x => x.Request.UserId).NotEmpty();
        }
    }
}
