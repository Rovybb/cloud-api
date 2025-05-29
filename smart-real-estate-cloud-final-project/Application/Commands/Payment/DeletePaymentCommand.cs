using Domain.Utils;
using MediatR;

namespace Application.Commands.Payment
{
    public class DeletePaymentCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }
}
