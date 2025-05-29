using Domain.Types.UserInformation;

namespace Domain.Entities
{
    public class UserInformation
    {

        // Legătura logică cu entitatea User din UsersDbContext
        public Guid Id { get; set; }

        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Address { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Nationality { get; set; }
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
        public required UserStatus Status { get; set; }
        public required UserRole Role { get; set; }

        // Câmpuri specifice conturilor profesionale (opționale)
        public string? Company { get; set; }
        public string? Type { get; set; }

        // Colecții asociate cu utilizatorul
        public IEnumerable<Property>? Properties { get; set; }
        public IEnumerable<Inquiry>? Inquiries { get; set; }
        public IEnumerable<Payment>? Payments { get; set; }
    }
}
