using Application.Commands.Payment;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.CommandHandlers.Payment
{
    public class DeletePaymentCommandHandler : IRequestHandler<DeletePaymentCommand, Result>
    {
        private readonly IPaymentRepository paymentRepository;

        public DeletePaymentCommandHandler(IPaymentRepository paymentRepository)
        {
            this.paymentRepository = paymentRepository;
        }

        public async Task<Result> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
        {
            var paymentResult = await paymentRepository.GetByIdAsync(request.Id);
            if (!paymentResult.IsSuccess)
            {
                return Result.Failure(paymentResult.ErrorMessage );
            }

            await paymentRepository.DeleteAsync(request.Id);
            return Result.Success();
        }
    }
}
