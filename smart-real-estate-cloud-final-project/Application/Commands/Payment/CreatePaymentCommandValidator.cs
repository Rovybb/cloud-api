using FluentValidation;

namespace Application.Commands.Payment
{
    public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentCommandValidator()
        {
            RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid payment type.");
            RuleFor(x => x.Date).NotEmpty().WithMessage("Payment date is required.");
            RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid payment status.");
            RuleFor(x => x.PaymentMethod).IsInEnum().WithMessage("Invalid payment method.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
            RuleFor(x => x.PropertyId).NotEmpty().WithMessage("Property ID is required.");
            RuleFor(x => x.SellerId).NotEmpty().WithMessage("Seller ID is required.");
            RuleFor(x => x.BuyerId).NotEmpty().WithMessage("Buyer ID is required.");
        }
    }
}
