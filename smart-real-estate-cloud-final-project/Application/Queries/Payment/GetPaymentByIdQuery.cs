using Application.DTOs;
using Domain.Utils;
using MediatR;

namespace Application.Queries.Payment
{
    public class GetPaymentByIdQuery : IRequest<Result<PaymentDto>>
    {
        public Guid Id { get; set; }
    }
}
