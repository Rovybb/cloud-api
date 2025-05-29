using Application.Commands.User;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.CommandHandlers.UserInformation
{
    public class UpdateUserInformationCommandHandler : IRequestHandler<UpdateUserInformationCommand, Result>
    {
        private readonly IUserInformationRepository userRepository;
        private readonly IMapper mapper;

        public UpdateUserInformationCommandHandler(IUserInformationRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(UpdateUserInformationCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await userRepository.GetByIdAsync(request.Id);
            if (!existingUser.IsSuccess)
            {
                return Result.Failure("User not found.");
            }

            mapper.Map(request.Request, existingUser.Data);

            var updateResult = await userRepository.UpdateAsync(existingUser.Data);
            if (updateResult.IsSuccess)
            {
                return Result.Success();
            }
            return Result.Failure(updateResult.ErrorMessage );
        }
    }
}
