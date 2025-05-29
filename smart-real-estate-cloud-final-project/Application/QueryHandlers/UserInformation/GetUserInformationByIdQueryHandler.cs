using Application.Queries.UserInformation;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using MediatR;
using Application.DTOs;

namespace Application.QueryHandlers.User
{
    public class GetUserInformationByIdQueryHandler : IRequestHandler<GetUserInformationByIdQuery, Result<UserDto>>
    {
        private readonly IUserInformationRepository userRepository;
        private readonly IMapper mapper;

        public GetUserInformationByIdQueryHandler(IUserInformationRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task<Result<UserDto>> Handle(GetUserInformationByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await userRepository.GetByIdAsync(request.Id);
            if (result.IsSuccess)
            {
                return Result<UserDto>.Success(mapper.Map<UserDto>(result.Data));
            }
            return Result<UserDto>.Failure(result.ErrorMessage );
        }
    }
}
