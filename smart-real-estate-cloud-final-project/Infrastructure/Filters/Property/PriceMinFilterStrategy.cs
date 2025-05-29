using Domain.Entities;

namespace Domain.Filters
{
    public class PriceMinFilterStrategy : IPropertyFilterStrategy
    {
        public IQueryable<Property> ApplyFilter(IQueryable<Property> query, string value)
        {
            if (decimal.TryParse(value, out var priceMin))
            {
                return query.Where(p => p.Price >= priceMin);
            }
            return query;
        }
    }
}
