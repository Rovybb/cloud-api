using Application.DTOs;
using Application.Queries.Inquiry;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.QueryHandlers.Inquiry
{
    public class GetAllInquiriesQueryHandler : IRequestHandler<GetAllInquiriesQuery, Result<IEnumerable<InquiryDto>>>
    {
        private readonly IInquiryRepository inquiryRepository;
        private readonly IMapper mapper;

        public GetAllInquiriesQueryHandler(IInquiryRepository inquiryRepository, IMapper mapper)
        {
            this.inquiryRepository = inquiryRepository;
            this.mapper = mapper;
        }

        public async Task<Result<IEnumerable<InquiryDto>>> Handle(GetAllInquiriesQuery request, CancellationToken cancellationToken)
        {
            var result = await inquiryRepository.GetAllAsync();
            if (result == null)
            {
                return Result<IEnumerable<InquiryDto>>.Failure("No inquiries found");
            }

            var inquirys = result.Select(inquiry => mapper.Map<InquiryDto>(inquiry));
            return Result<IEnumerable<InquiryDto>>.Success(inquirys);
        }
    }
}
