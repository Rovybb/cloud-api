using Domain.Entities;
using Domain.Repositories;
using Domain.Utils;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class InquiryRepository : IInquiryRepository
    {
        private readonly ApplicationDbContext context;

        public InquiryRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<Result<Inquiry>> GetByIdAsync(Guid id)
        {
            try
            {
                var inquiry = await context.Inquiries.FirstOrDefaultAsync(x => x.Id == id);
                if (inquiry == null)
                {
                    return Result<Inquiry>.Failure("Inquiry not found");
                }
                return Result<Inquiry>.Success(inquiry);
            }
            catch (Exception ex)
            {
                return Result<Inquiry>.Failure(ex.Message);
            }
        }

        public async Task<Result<Guid>> CreateAsync(Inquiry inquiry)
        {
            try
            {
                await context.Inquiries.AddAsync(inquiry);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(inquiry.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }

        public async Task<Result> UpdateAsync(Inquiry inquiry)
        {
            try
            {
                var existingInquiry = await context.Inquiries.FindAsync(inquiry.Id);
                if (existingInquiry == null)
                {
                    return Result.Failure("Inquiry not found.");
                }

                context.Entry(existingInquiry).CurrentValues.SetValues(inquiry);
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
                var inquiry = await context.Inquiries.FirstOrDefaultAsync(x => x.Id == id);
                if (inquiry == null)
                {
                    return Result.Failure("Inquiry not found.");
                }

                context.Inquiries.Remove(inquiry);
                await context.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<IEnumerable<Inquiry>> GetAllAsync()
        {
            return await context.Inquiries.ToListAsync();
        }
    }
}
