using Domain.Types.Payment;
using Domain.Utils;
using MediatR;

namespace Application.Commands.Payment
{
    public class CreatePaymentCommand : IRequest<Result<Guid>>
    {
        public required PaymentType Type { get; set; }
        public required decimal Price { get; set; }
        public required DateTime Date { get; set; }
        public required PaymentStatus Status { get; set; }
        public required PaymentMethod PaymentMethod { get; set; }
        public required Guid PropertyId { get; set; }
        public required Guid SellerId { get; set; }
        public required Guid BuyerId { get; set; }

    }
}
