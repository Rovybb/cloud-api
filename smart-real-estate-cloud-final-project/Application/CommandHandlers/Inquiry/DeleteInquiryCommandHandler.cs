using Application.Commands.Inquiry;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.CommandHandlers.Inquiry
{
    public class DeleteInquiryCommandHandler : IRequestHandler<DeleteInquiryCommand, Result>
    {
        private readonly IInquiryRepository inquiryRepository;

        public DeleteInquiryCommandHandler(IInquiryRepository inquiryRepository)
        {
            this.inquiryRepository = inquiryRepository;
        }

        public async Task<Result> Handle(DeleteInquiryCommand request, CancellationToken cancellationToken)
        {
            var result = await inquiryRepository.DeleteAsync(request.Id);
            if (result.IsSuccess)
            {
                return Result.Success();
            }
            return Result.Failure(result.ErrorMessage ?? "Unknown error occurred.");
        }
    }
}
