using Application.Contracts.Property;
using Domain.Utils;
using MediatR;

namespace Application.Commands.Property
{
    public class UpdatePropertyCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public required UpdatePropertyRequest Request { get; set; }
    }
}
