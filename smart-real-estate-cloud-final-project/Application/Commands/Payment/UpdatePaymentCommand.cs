using Application.Contracts.Payment;
using Domain.Types.Payment;
using Domain.Utils;
using MediatR;

namespace Application.Commands.Payment
{
    public class UpdatePaymentCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public required UpdatePaymentRequest Request { get; set; }
    }
}
