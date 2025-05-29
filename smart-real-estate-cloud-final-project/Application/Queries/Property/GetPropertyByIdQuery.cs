using Application.DTOs;
using Domain.Utils;
using MediatR;

namespace Application.Queries.Property
{
    public class GetPropertyByIdQuery : IRequest<Result<PropertyDto>>
    {
        public Guid Id { get; set; }
    }
}
