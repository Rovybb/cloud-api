using Application.DTOs;
using Domain.Utils;
using MediatR;

namespace Application.Queries.Payment
{
    public class GetAllPaymentsQuery : IRequest<Result<IEnumerable<PaymentDto>>>
    {
    }
}
