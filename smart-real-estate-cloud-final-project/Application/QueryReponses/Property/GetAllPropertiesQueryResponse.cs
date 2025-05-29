using Application.DTOs;
using Domain.Utils;

namespace Application.QueryReponses.Property
{
    public class GetAllPropertiesQueryResponse
    {
        public PaginatedList<PropertyDto>? Items { get; set; }
        public int TotalPages { get; set; }
    }
}
