using Application.Commands.Payment;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.CommandHandlers.Payment
{
    public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, Result>
    {
        private readonly IPaymentRepository paymentRepository;
        private readonly IMapper mapper;

        public UpdatePaymentCommandHandler(IPaymentRepository paymentRepository, IMapper mapper)
        {
            this.paymentRepository = paymentRepository;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
        {
            var existingPayment = await paymentRepository.GetByIdAsync(request.Id);
            if (!existingPayment.IsSuccess)
            {
                return Result.Failure("Payment not found.");
            }

            mapper.Map(request.Request, existingPayment.Data);

            var updateResult = await paymentRepository.UpdateAsync(existingPayment.Data);
            if (updateResult.IsSuccess)
            {
                return Result.Success();
            }
            return Result.Failure(updateResult.ErrorMessage );
        }
    }
}
