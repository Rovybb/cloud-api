using Application.Commands.User;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.CommandHandlers.UserInformation
{
    public class CreateUserInformationCommandHandler : IRequestHandler<CreateUserInformationCommand, Result<Guid>>
    {
        private readonly IUserInformationRepository userRepository;
        private readonly IMapper mapper;

        public CreateUserInformationCommandHandler(IUserInformationRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateUserInformationCommand request, CancellationToken cancellationToken)
        {
            var result = await userRepository.CreateAsync(mapper.Map<Domain.Entities.UserInformation>(request));
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage );
        }
    }
}
