using Domain.Entities;

namespace Domain.Filters
{
    public class PriceMaxFilterStrategy : IPropertyFilterStrategy
    {
        public IQueryable<Property> ApplyFilter(IQueryable<Property> query, string value)
        {
            if (decimal.TryParse(value, out var priceMax))
            {
                return query.Where(p => p.Price <= priceMax);
            }
            return query;
        }
    }
}
