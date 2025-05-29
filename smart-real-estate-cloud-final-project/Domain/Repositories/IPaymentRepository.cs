using Domain.Entities;
using Domain.Utils;

namespace Domain.Repositories
{
    public interface IPaymentRepository
    {
        Task<Result<Payment>> GetByIdAsync(Guid id);
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Result<Guid>> CreateAsync(Payment payment);
        Task<Result> UpdateAsync(Payment payment);
        Task<Result> DeleteAsync(Guid id);

    }
}
