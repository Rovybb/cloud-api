using Application.Commands.Payment;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Domain.Types.Payment;
using Domain.Utils;
using MediatR;

namespace Application.CommandHandlers.Payment
{
    public class CreateStripeCheckoutCommandHandler
        : IRequestHandler<CreateCheckoutCommand, Result<string>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICheckoutService _stripeCheckoutService;
        private readonly IMapper _mapper;

        public CreateStripeCheckoutCommandHandler(
            IPaymentRepository paymentRepository,
            ICheckoutService stripeCheckoutService,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _stripeCheckoutService = stripeCheckoutService;
            _mapper = mapper;
        }

        public async Task<Result<string>> Handle(
            CreateCheckoutCommand request,
            CancellationToken cancellationToken)
        {
            var paymentEntity = _mapper.Map<Domain.Entities.Payment>(request);
            paymentEntity.Status = PaymentStatus.PENDING;
            paymentEntity.Date = DateTime.UtcNow;

            var createResult = await _paymentRepository.CreateAsync(paymentEntity);
            if (!createResult.IsSuccess)
            {
                return Result<string>.Failure(createResult.ErrorMessage);
            }

            try
            {
                var checkoutUrl = await _stripeCheckoutService.CreateCheckoutSessionAsync(
                    amount: request.Price,
                    currency: "usd", // or "eur", etc.
                    successUrl: request.SuccessUrl,
                    cancelUrl: request.CancelUrl
                );

                return Result<string>.Success(checkoutUrl);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(ex.Message);
            }
        }
    }
}
