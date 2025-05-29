using Domain.Types.Inquiry;

namespace Application.Contracts.Inquiry
{
    public class UpdateInquiryRequest
    {
        public required string Message { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required InquiryStatus Status { get; set; }
        public required Guid PropertyId { get; set; }
        public required Guid ClientId { get; set; }
        public required Guid AgentId { get; set; }
    }
}
