using Application.DTOs;
using Domain.Utils;
using MediatR;

namespace Application.Queries.Inquiry
{
    public class GetAllInquiriesQuery : IRequest<Result<IEnumerable<InquiryDto>>>
    {
    }
}
