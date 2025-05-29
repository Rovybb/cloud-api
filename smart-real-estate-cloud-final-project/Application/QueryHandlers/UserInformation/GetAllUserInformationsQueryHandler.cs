using Application.Queries.UserInformation;
using AutoMapper;
using Domain.Repositories;
using MediatR;
using Domain.Utils;
using Application.DTOs;

namespace Application.QueryHandlers.User
{
    public class GetAllUserInformationsQueryHandler : IRequestHandler<GetAllUserInformationsQuery, Result<IEnumerable<UserDto>>>
    {
        private readonly IUserInformationRepository userRepository;
        private readonly IMapper mapper;

        public GetAllUserInformationsQueryHandler(IUserInformationRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task<Result<IEnumerable<UserDto>>> Handle(GetAllUserInformationsQuery request, CancellationToken cancellationToken)
        {
            var result = await userRepository.GetAllAsync();
            var users = result.Select(user => mapper.Map<UserDto>(user));
            return Result<IEnumerable<UserDto>>.Success(users);
        }
    }
}
