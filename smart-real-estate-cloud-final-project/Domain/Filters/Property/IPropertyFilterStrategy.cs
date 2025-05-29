using Domain.Entities;

namespace Domain.Filters
{
    public interface IPropertyFilterStrategy
    {
        IQueryable<Property> ApplyFilter(IQueryable<Property> query, string value);
    }
}
