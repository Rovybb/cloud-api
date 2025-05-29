using Application.DTOs;
using Application.Queries.Property;
using Application.QueryReponses.Property;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.QueryHandlers.Property
{
    public class GetAllPropertiesQueryHandler : IRequestHandler<GetAllPropertiesQuery, Result<GetAllPropertiesQueryResponse>>
    {
        private readonly IPropertyRepository propertyRepository;
        private readonly IMapper mapper;

        public GetAllPropertiesQueryHandler(IPropertyRepository propertyRepository, IMapper mapper)
        {
            this.propertyRepository = propertyRepository;
            this.mapper = mapper;
        }

        public async Task<Result<GetAllPropertiesQueryResponse>> Handle(GetAllPropertiesQuery request, CancellationToken cancellationToken)
        {
            var result = await propertyRepository.GetPropertiesAsync(request.PageNumber, request.PageSize, request.Filters);

            if (result == null)
            {
                return Result<GetAllPropertiesQueryResponse>.Failure("No properties!");
            }

            var response = new GetAllPropertiesQueryResponse
            {
                Items = mapper.Map<PaginatedList<PropertyDto>>(result),
                TotalPages = result.TotalPages
            };
            return Result<GetAllPropertiesQueryResponse>.Success(response);
        }
    }

}
