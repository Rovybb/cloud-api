using Application.Commands.Inquiry;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.CommandHandlers.Inquiry
{
    public class UpdateInquiryCommandHandler : IRequestHandler<UpdateInquiryCommand, Result>
    {
        private readonly IInquiryRepository inquiryRepository;
        private readonly IMapper mapper;

        public UpdateInquiryCommandHandler(IInquiryRepository inquiryRepository, IMapper mapper)
        {
            this.inquiryRepository = inquiryRepository;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(UpdateInquiryCommand request, CancellationToken cancellationToken)
        {
            var existingInquiry = await inquiryRepository.GetByIdAsync(request.Id);
            if (!existingInquiry.IsSuccess)
            {
                return Result.Failure("Inquiry not found.");
            }

            mapper.Map(request.Request, existingInquiry.Data);

            var updateResult = await inquiryRepository.UpdateAsync(existingInquiry.Data);
            if (updateResult.IsSuccess)
            {
                return Result.Success();
            }
            return Result.Failure(updateResult.ErrorMessage );


        }
    }
}
