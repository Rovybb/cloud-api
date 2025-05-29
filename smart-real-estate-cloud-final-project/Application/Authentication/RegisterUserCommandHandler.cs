using Domain.Entities;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    private readonly IUserRepository userRepository;
    private readonly IUserInformationRepository userInfoRepository;

    public RegisterUserCommandHandler(IUserRepository userRepository, IUserInformationRepository userInfoRepository)
    {
        this.userRepository = userRepository;
        this.userInfoRepository = userInfoRepository;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Verificăm dacă user-ul există deja
        var existingUser = await userRepository.GetByEmail(request.Email, cancellationToken);
        if (existingUser.IsSuccess)
        {
            return Result<Guid>.Failure("Email already exists!");
        }

        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        // Înregistrăm user-ul în UsersDbContext
        var registerResult = await userRepository.Register(user, cancellationToken);
        if (!registerResult.IsSuccess)
        {
            // Dacă nu s-a putut crea user-ul, returnăm eroarea
            return Result<Guid>.Failure(registerResult.ErrorMessage);
        }

        var userId = registerResult.Data; // Id-ul user-ului creat

        // Creăm intrarea UserInformation în ApplicationDbContext
        var userInfo = new UserInformation
        {
            Id = userId, // Legătură logică la entitatea User din celălalt context
            Username = "",
            FirstName = "",
            LastName = "",
            Email = request.Email,
            Address = "",
            PhoneNumber = "",
            Nationality = "",
            CreatedAt = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow,
            Status = 0,
            Role = 0,
            Company = "",
            Type = ""
        };

        var infoResult = await userInfoRepository.CreateAsync(userInfo);
        if (!infoResult.IsSuccess)
        {
            // Dacă nu s-a putut crea UserInformation, returnăm eroarea
            return Result<Guid>.Failure(infoResult.ErrorMessage);
        }

        // Returnăm Id-ul user-ului creat cu succes
        return Result<Guid>.Success(userId);
    }
}
