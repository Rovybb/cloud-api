using Domain.Utils;
using MediatR;

namespace Application.Commands.Property
{
    public class DeletePropertyImageCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }
}
