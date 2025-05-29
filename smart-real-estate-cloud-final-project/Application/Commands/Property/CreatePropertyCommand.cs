using Domain.Utils;
using MediatR;


namespace Application.Commands.Property
{
    public class CreatePropertyCommand : IRequest<Result<Guid>>
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        //public required PropertyType Type { get; set; }
        //public required PropertyStatus Status { get; set; }
        public required string Address { get; set; }
        public required decimal Price { get; set; }
        public required decimal Area { get; set; }
        public required int Rooms { get; set; }
        public required int Bathrooms { get; set; }
        public required int ConstructionYear { get; set; }
        public required Guid UserId { get; set; }
    }
}
