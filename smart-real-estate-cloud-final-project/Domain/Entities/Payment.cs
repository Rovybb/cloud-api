using Domain.Types.Payment;

namespace Domain.Entities
{
    public class Payment
    {
        public required Guid Id { get; set; }
        public required PaymentType Type { get; set; }
        public required DateTime Date { get; set; }
        public required decimal Price { get; set; }
        public required PaymentStatus Status { get; set; }
        public required PaymentMethod PaymentMethod { get; set; }
        public required Guid PropertyId { get; set; }
        public required Property Property { get; set; }
        public required Guid SellerId { get; set; }
        public required UserInformation Seller { get; set; }
        public required Guid BuyerId { get; set; }
        public required UserInformation Buyer { get; set; }
    }
}
