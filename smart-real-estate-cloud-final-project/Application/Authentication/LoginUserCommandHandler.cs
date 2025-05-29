using Domain.Entities;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<string>>
{
    private readonly IUserRepository userRepository;

    public LoginUserCommandHandler(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Email = request.Email,
            PasswordHash = request.Password
        };
        return await userRepository.Login(user, cancellationToken);
    }
}
