using Application.Contracts.Inquiry;
using Domain.Utils;
using MediatR;

namespace Application.Commands.Inquiry
{
    public class UpdateInquiryCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public required UpdateInquiryRequest Request { get; set; }
    }
}
