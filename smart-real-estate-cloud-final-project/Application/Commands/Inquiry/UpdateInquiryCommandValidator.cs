using FluentValidation;

namespace Application.Commands.Inquiry
{
    public class UpdateInquiryCommandValidator : AbstractValidator<UpdateInquiryCommand>
    {
        public UpdateInquiryCommandValidator()
        {
            RuleFor(x => x.Request.Message).NotEmpty().WithMessage("Message is required.");
            RuleFor(x => x.Request.Status).IsInEnum().WithMessage("Status must be a valid enum value.");
            RuleFor(x => x.Request.PropertyId).NotEmpty().WithMessage("PropertyId is required.");
            RuleFor(x => x.Request.CreatedAt).NotEmpty().WithMessage("CreatedAt is required.");
            RuleFor(x => x.Request.ClientId).NotEmpty().WithMessage("ClientId is required.");
            RuleFor(x => x.Request.AgentId).NotEmpty().WithMessage("AgentId is required.");
        }
    }
}
