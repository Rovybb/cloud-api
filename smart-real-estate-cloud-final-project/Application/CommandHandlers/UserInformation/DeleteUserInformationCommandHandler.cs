using Application.Commands.User;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.CommandHandlers.UserInformation
{
    public class DeleteUserInformationCommandHandler : IRequestHandler<DeleteUserInformationCommand, Result>
    {
        private readonly IUserInformationRepository userRepository;

        public DeleteUserInformationCommandHandler(IUserInformationRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Result> Handle(DeleteUserInformationCommand request, CancellationToken cancellationToken)
        {
            var result = await userRepository.DeleteAsync(request.Id);
            if (result.IsSuccess)
            {
                return Result.Success();
            }
            return Result.Failure(result.ErrorMessage );
        }
    }
}
