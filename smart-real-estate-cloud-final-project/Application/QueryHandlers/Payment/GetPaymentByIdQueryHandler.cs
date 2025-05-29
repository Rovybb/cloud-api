using Application.DTOs;
using Application.Queries.Payment;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.QueryHandlers.Payment
{
    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, Result<PaymentDto>>
    {
        private readonly IPaymentRepository paymentRepository;
        private readonly IMapper mapper;

        public GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
        {
            this.paymentRepository = paymentRepository;
            this.mapper = mapper;
        }

        public async Task<Result<PaymentDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await paymentRepository.GetByIdAsync(request.Id);
            if (result.IsSuccess)
            {
                return Result<PaymentDto>.Success(mapper.Map<PaymentDto>(result.Data));
            }
            return Result<PaymentDto>.Failure(result.ErrorMessage );
        }
    }
}
