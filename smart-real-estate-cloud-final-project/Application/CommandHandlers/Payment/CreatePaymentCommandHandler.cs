using Application.Commands.Payment;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.CommandHandlers.Payment
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<Guid>>
    {
        private readonly IPaymentRepository paymentRepository;
        private readonly IMapper mapper;

        public CreatePaymentCommandHandler(IPaymentRepository paymentRepository, IMapper mapper)
        {
            this.paymentRepository = paymentRepository;
            this.mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var result = await paymentRepository.CreateAsync(mapper.Map<Domain.Entities.Payment>(request));
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage );
        }
    }
}
