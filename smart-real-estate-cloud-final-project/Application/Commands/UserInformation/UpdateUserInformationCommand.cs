using Application.Contracts.UserInformation;
using Domain.Utils;
using MediatR;

namespace Application.Commands.User
{
    public class UpdateUserInformationCommand : IRequest<Result>
    {
        public required Guid Id { get; set; }
        
        public required UpdateUserInformationRequest Request { get; set; }
    }
}
