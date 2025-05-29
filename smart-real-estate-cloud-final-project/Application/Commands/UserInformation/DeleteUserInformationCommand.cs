using Domain.Utils;
using MediatR;

namespace Application.Commands.User
{
    public class DeleteUserInformationCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }
}
