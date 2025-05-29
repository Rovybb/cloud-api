using Domain.Types.Inquiry;

namespace Domain.Entities
{
    public class Inquiry
    {
        public required Guid Id { get; set; }
        public required string Message { get; set; }
        public required InquiryStatus Status { get; set; }
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public required Guid PropertyId { get; set; }
        public required Property Property { get; set; }
        public required Guid ClientId { get; set; }
        public required UserInformation Client { get; set; }
        public required Guid AgentId { get; set; }
        public required UserInformation Agent { get; set; }
    }
}
