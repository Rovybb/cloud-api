using FluentValidation;

namespace Application.Commands.Payment
{
    public class UpdatePaymentCommandValidator : AbstractValidator<UpdatePaymentCommand>
    {
        public UpdatePaymentCommandValidator() {
            RuleFor(x => x.Request.Type).IsInEnum().WithMessage("Invalid payment type.");
            RuleFor(x => x.Request.Date).NotEmpty().WithMessage("Payment date is required.");
            RuleFor(x => x.Request.Status).IsInEnum().WithMessage("Invalid payment status.");
            RuleFor(x => x.Request.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
            RuleFor(x => x.Request.PaymentMethod).IsInEnum().WithMessage("Invalid payment method.");
            RuleFor(x => x.Request.PropertyId).NotEmpty().WithMessage("Property ID is required.");
            RuleFor(x => x.Request.BuyerId).NotEmpty().WithMessage("Buyer ID is required.");
            RuleFor(x => x.Request.SellerId).NotEmpty().WithMessage("Seller ID is required.");
        }
    }
}
