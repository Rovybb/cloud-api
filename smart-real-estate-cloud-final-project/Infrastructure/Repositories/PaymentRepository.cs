using Domain.Entities;
using Domain.Repositories;
using Domain.Utils;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext context;

        public PaymentRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<Result<Payment>> GetByIdAsync(Guid id)
        {
            try
            {
                var payment = await context.Payments.FirstOrDefaultAsync(x => x.Id == id);
                if (payment == null)
                {
                    return Result<Payment>.Failure("Payment not found");
                }
                return Result<Payment>.Success(payment);
            }
            catch (Exception ex)
            {
                return Result<Payment>.Failure(ex.Message);
            }
        }

        public async Task<Result<Guid>> CreateAsync(Payment payment)
        {
            try
            { 
                await context.Payments.AddAsync(payment);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(payment.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }

        public async Task<Result> UpdateAsync(Payment payment)
        {
            try
            {
                var existingPayment = await context.Payments.FindAsync(payment.Id);
                if (existingPayment == null)
                {
                    return Result.Failure("Payment not found.");
                }

                context.Entry(existingPayment).CurrentValues.SetValues(payment);
                await context.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            try
            {
                var payment = await context.Payments.FirstOrDefaultAsync(x => x.Id == id);
                if (payment == null)
                {
                    return Result.Failure("Payment not found.");
                }

                context.Payments.Remove(payment);
                await context.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await context.Payments.ToListAsync();
        }
    }
}
