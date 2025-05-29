using Domain.Entities;

namespace Domain.Filters
{
    public class DescriptionFilterStrategy : IPropertyFilterStrategy
    {
        public IQueryable<Property> ApplyFilter(IQueryable<Property> query, string value)
        {
            return query.Where(p => p.Description.Contains(value));
        }
    }
}
