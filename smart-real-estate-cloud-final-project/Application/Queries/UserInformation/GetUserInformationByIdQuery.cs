using Application.DTOs;
using Domain.Utils;
using MediatR;

namespace Application.Queries.UserInformation
{
    public class GetUserInformationByIdQuery : IRequest<Result<UserDto>>
    {
        public Guid Id { get; set; }
    }
}
