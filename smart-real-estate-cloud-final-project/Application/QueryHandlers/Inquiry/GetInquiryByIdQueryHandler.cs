using Application.DTOs;
using Application.Queries.Inquiry;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.QueryHandlers.Inquiry
{
    public class GetInquiryByIdQueryHandler : IRequestHandler<GetInquiryByIdQuery, Result<InquiryDto>>
    {
        private readonly IInquiryRepository inquiryRepository;
        private readonly IMapper mapper;

        public GetInquiryByIdQueryHandler(IInquiryRepository inquiryRepository, IMapper mapper)
        {
            this.inquiryRepository = inquiryRepository;
            this.mapper = mapper;
        }

        public async Task<Result<InquiryDto>> Handle(GetInquiryByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await inquiryRepository.GetByIdAsync(request.Id);
            if (result.IsSuccess)
            {
                return Result<InquiryDto>.Success(mapper.Map<InquiryDto>(result.Data));
            }
            return Result<InquiryDto>.Failure(result.ErrorMessage );
        }
    }
}
