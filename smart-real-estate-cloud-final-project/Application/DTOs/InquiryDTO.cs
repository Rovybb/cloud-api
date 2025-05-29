using Domain.Types.Inquiry;

namespace Application.DTOs
{
    public class InquiryDto
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public Guid AgentId { get; set; }
        public Guid ClientId { get; set; }
        public required string Message { get; set; } 
        public InquiryStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
