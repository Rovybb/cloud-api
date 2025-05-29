using Domain.Entities;
using Domain.Utils;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        Task<Result<Guid>> Register(User user, CancellationToken cancellationToken);
        Task<Result<string>> Login(User user, CancellationToken cancellationToken);
        Task<Result<User>> GetByEmail(string email, CancellationToken cancellationToken);
    }
}
