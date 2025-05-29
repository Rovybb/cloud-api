using Domain.Types.Payment;

namespace Application.DTOs
{
    public class PaymentDto
    {
        public required Guid Id { get; set; }
        public required PaymentType Type { get; set; }
        public required DateTime Date { get; set; }
        public required decimal Price { get; set; }
        public required PaymentMethod PaymentMethod { get; set; }
        public required PaymentStatus Status { get; set; }
        public required Guid PropertyId { get; set; }
        public required Guid SellerId { get; set; }
        public required Guid BuyerId { get; set; }
    }
}
