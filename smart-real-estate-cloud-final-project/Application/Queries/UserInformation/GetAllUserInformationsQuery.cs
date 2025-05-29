using Application.DTOs;
using Domain.Utils;
using MediatR;

namespace Application.Queries.UserInformation
{
    public class GetAllUserInformationsQuery : IRequest<Result<IEnumerable<UserDto>>>
    {
    }
}
