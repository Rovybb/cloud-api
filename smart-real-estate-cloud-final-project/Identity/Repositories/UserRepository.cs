using Domain.Entities;
using Domain.Repositories;
using Domain.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UsersDbContext usersDbContext;
        private readonly IConfiguration configuration;

        public UserRepository(UsersDbContext usersDbContext, IConfiguration configuration)
        {
            this.usersDbContext = usersDbContext;
            this.configuration = configuration;
        }

        public async Task<Result<string>> Login(User user, CancellationToken cancellationToken)
        {
            var existingUser = await usersDbContext.Users.SingleOrDefaultAsync(u => u.Email == user.Email, cancellationToken);
            if (existingUser == null)
            {
                return Result<string>.Failure("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(user.PasswordHash, existingUser.PasswordHash))
            {
                return Result<string>.Failure("Invalid password.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, existingUser    .Id.ToString()) }), 
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Result<string>.Success(tokenHandler.WriteToken(token));
        }

        public async Task<Result<Guid>> Register(User user, CancellationToken cancellationToken)
        {
            try
            {
                await usersDbContext.Users.AddAsync(user, cancellationToken);
                await usersDbContext.SaveChangesAsync(cancellationToken);
                return Result<Guid>.Success(user.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }

        public async Task<Result<User>> GetByEmail(string email, CancellationToken cancellationToken)
        {
            var user = await usersDbContext.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
            return user == null ? Result<User>.Failure("User not found.") : Result<User>.Success(user);
        }
    }
}
