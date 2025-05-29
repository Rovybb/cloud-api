using Application.DTOs;
using Application.Queries.Payment;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.QueryHandlers.Payment
{
    public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, Result<IEnumerable<PaymentDto>>>
    {
        private readonly IPaymentRepository paymentRepository;
        private readonly IMapper mapper;

        public GetAllPaymentsQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
        {
            this.paymentRepository = paymentRepository;
            this.mapper = mapper;
        }

        public async Task<Result<IEnumerable<PaymentDto>>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
        {
            var payments = await paymentRepository.GetAllAsync();
            if (payments == null)
            {
                return Result<IEnumerable<PaymentDto>>.Failure("No payments found.");
            }

            var paymentDTOs = mapper.Map<IEnumerable<PaymentDto>>(payments);
            return Result<IEnumerable<PaymentDto>>.Success(paymentDTOs);
        }
    }
}
