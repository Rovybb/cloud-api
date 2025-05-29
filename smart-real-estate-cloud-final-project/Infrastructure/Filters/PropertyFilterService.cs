using Domain.Filters;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services
{
    public class PropertyFilterService
    {
        private readonly IServiceProvider _serviceProvider;

        public PropertyFilterService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IQueryable<Property> ApplyFilters(IQueryable<Property> query, Dictionary<string, string>? filters)
        {
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    var filterStrategy = GetFilterStrategy(filter.Key.ToLower());
                    if (filterStrategy != null)
                    {
                        query = filterStrategy.ApplyFilter(query, filter.Value);
                    }
                }
            }

            return query;
        }

        private IPropertyFilterStrategy? GetFilterStrategy(string filterKey)
        {
            return filterKey switch
            {
                "title" => _serviceProvider.GetRequiredService<TitleFilterStrategy>(),
                "price_min" => _serviceProvider.GetRequiredService<PriceMinFilterStrategy>(),
                "price_max" => _serviceProvider.GetRequiredService<PriceMaxFilterStrategy>(),
                "description" => _serviceProvider.GetRequiredService<DescriptionFilterStrategy>(),
                _ => null
            };
        }
    }
}
