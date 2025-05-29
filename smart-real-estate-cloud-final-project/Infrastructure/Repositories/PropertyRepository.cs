using Domain.Entities;
using Domain.Repositories;
using Domain.Utils;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly ApplicationDbContext context;
        private readonly PropertyFilterService filterService;

        public PropertyRepository(ApplicationDbContext context, PropertyFilterService filterService)
        {
            this.context = context;
            this.filterService = filterService;
        }

        public async Task<PaginatedList<Property>> GetPropertiesAsync(
        int pageNumber,
        int pageSize,
        Dictionary<string, string>? filters)
        {
            // Start by including images
            var query = context.Properties
                .Include(p => p.PropertyImages)
                .AsQueryable();

            // Apply filtering
            query = filterService.ApplyFilters(query, filters);

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<Property>(items, totalItems, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Property>> GetAllAsync()
        {
            return await context.Properties.ToListAsync();
        }

        public async Task<Result<Property>> GetByIdAsync(Guid id)
        {
            // Instead of context.Properties.FindAsync(id), do:
            var property = await context.Properties
                .Include(p => p.PropertyImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                return Result<Property>.Failure("Property not found.");
            }

            return Result<Property>.Success(property);
        }


        public async Task<Result<Guid>> CreateAsync(Property property)
        {
            try
            {
                await context.Properties.AddAsync(property);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(property.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }

        public async Task<Result> UpdateAsync(Property property)
        {
            try
            {
                var existingProperty = await context.Properties.FindAsync(property.Id);
                if (existingProperty == null)
                {
                    return Result.Failure("Property not found.");
                }

                context.Entry(existingProperty).CurrentValues.SetValues(property);
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
            var property = await context.Properties.FirstOrDefaultAsync(x => x.Id == id);
            if (property == null)
            {
                return Result.Failure("Property not found.");
            }

            context.Properties.Remove(property);
            await context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> AddImageAsync(Guid propertyId, PropertyImage image)
        {
            try
            {
                // Load the property with its existing images
                var property = await context.Properties
                    .Include(p => p.PropertyImages)
                    .FirstOrDefaultAsync(p => p.Id == propertyId);

                if (property == null)
                {
                    return Result.Failure("Property not found.");
                }

                // Associate the image with this property
                // If not already set:
                image.PropertyId = propertyId;

                await context.PropertyImages.AddAsync(image);

                // Add to the collection
                property.PropertyImages.Add(image);


                // Save changes
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return Result.Failure(ex.InnerException?.Message ?? ex.Message);
                }
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<PropertyImage>> GetImageByIdAsync(Guid imageId)
        {
            var image = await context.PropertyImages.FirstOrDefaultAsync(x => x.Id == imageId);
            if (image == null)
            {
                return Result<PropertyImage>.Failure("Image not found!");
            }
            return Result<PropertyImage>.Success(image);
        }

        public async Task<Result> RemoveImageAsync(Guid imageId)
        {
            var image = await context.PropertyImages.FirstOrDefaultAsync(x => x.Id == imageId);
            if (image == null)
            {
                return Result.Failure("Image not found.");
            }

            context.PropertyImages.Remove(image);
            await context.SaveChangesAsync();
            return Result.Success();
        }
    }
}
