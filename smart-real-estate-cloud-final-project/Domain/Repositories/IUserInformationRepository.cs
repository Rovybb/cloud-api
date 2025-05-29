using Domain.Entities;
using Domain.Utils;

namespace Domain.Repositories
{
    public interface IUserInformationRepository
    {
        Task<Result<UserInformation>> GetByIdAsync(Guid id);
        Task<IEnumerable<UserInformation>> GetAllAsync();
        Task<Result<Guid>> CreateAsync(UserInformation user);
        Task<Result<Guid>> UpdateAsync(UserInformation user);
        Task<Result> DeleteAsync(Guid id);
    }
}
