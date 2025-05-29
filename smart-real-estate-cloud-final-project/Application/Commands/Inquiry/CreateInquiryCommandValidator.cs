using FluentValidation;

namespace Application.Commands.Inquiry
{
    public class CreateInquiryCommandValidator : AbstractValidator<CreateInquiryCommand>
    {
        public CreateInquiryCommandValidator()
        {
            RuleFor(x => x.Message).NotEmpty().WithMessage("Message is required.");
            RuleFor(x => x.PropertyId).NotEmpty().WithMessage("PropertyId is required.");
            RuleFor(x => x.ClientId).NotEmpty().WithMessage("ClientId is required.");
            RuleFor(x => x.Status).IsInEnum().WithMessage("Status must be a valid enum value.");
            RuleFor(x => x.AgentId).NotEmpty().WithMessage("AgentId is required.");
        }
    }
}
