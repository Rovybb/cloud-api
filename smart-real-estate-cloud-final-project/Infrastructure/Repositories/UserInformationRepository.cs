using Domain.Entities;
using Domain.Repositories;
using Domain.Utils;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserInformationRepository : IUserInformationRepository
    {
        private readonly ApplicationDbContext context;

        public UserInformationRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<UserInformation>> GetAllAsync()
        {
            return await context.Users.ToListAsync();
        }

        public async Task<Result<UserInformation>> GetByIdAsync(Guid id)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                {
                    return Result<UserInformation>.Failure("User not found");
                }
                return Result<UserInformation>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<UserInformation>.Failure(ex.Message);
            }
        }

        public async Task<Result<Guid>> CreateAsync(UserInformation user)
        {
            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(user.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }

        public async Task<Result<Guid>> UpdateAsync(UserInformation user)
        {
            try
            {
                var existingUser = await context.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    return Result<Guid>.Failure("User not found.");
                }

                context.Entry(existingUser).CurrentValues.SetValues(user);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(existingUser.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                {
                    return Result.Failure("User not found.");
                }

                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }
    }
}
