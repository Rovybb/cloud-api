using Domain.Types.Inquiry;
using Domain.Utils;
using MediatR;

namespace Application.Commands.Inquiry
{
    public class CreateInquiryCommand : IRequest<Result<Guid>>
    {
        public required string Message { get; set; }
        public required InquiryStatus Status { get; set; }

        public required Guid PropertyId { get; set; }
        public required Guid AgentId { get; set; }
        public required Guid ClientId { get; set; }
    }
}
