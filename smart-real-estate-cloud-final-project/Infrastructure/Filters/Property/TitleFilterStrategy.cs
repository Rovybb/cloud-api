using Domain.Entities;

namespace Domain.Filters
{
    public class TitleFilterStrategy : IPropertyFilterStrategy
    {
        public IQueryable<Property> ApplyFilter(IQueryable<Property> query, string value)
        {
            return query.Where(p => p.Title.Contains(value));
        }
    }
}
