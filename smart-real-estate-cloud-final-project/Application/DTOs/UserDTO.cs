using Domain.Types.UserInformation;

namespace Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string LastName { get; set; }
        public required string FirstName { get; set; }
        public required string Address { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Nationality { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public UserStatus Status { get; set; }
        public UserRole Role { get; set; }
    }
}
