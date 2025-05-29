using Domain.Types.UserInformation;
using Domain.Utils;
using MediatR;

namespace Application.Commands.User
{
    public class CreateUserInformationCommand : IRequest<Result<Guid>>
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Address { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Nationality { get; set; }
        public DateTime? LastLogin { get; set; } 
        public required UserStatus Status { get; set; }
        public required UserRole Role { get; set; } 
        public string? Company { get; set; } 
        public string? Type { get; set; } 

    }
}
