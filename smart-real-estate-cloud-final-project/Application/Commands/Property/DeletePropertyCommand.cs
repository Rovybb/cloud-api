using Domain.Utils;
using MediatR;

namespace Application.Commands.Property
{
    public class DeletePropertyCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }
}
