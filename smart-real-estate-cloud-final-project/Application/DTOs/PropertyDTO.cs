using Domain.Types.Property;

namespace Application.DTOs
{
    public class PropertyDto
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required PropertyType Type { get; set; }
        public required PropertyStatus Status { get; set; }
        public required decimal Price { get; set; }
        public required decimal Area { get; set; }
        public required string Address { get; set; }
        public required int Rooms { get; set; }
        public required int Bathrooms { get; set; }
        public required int ConstructionYear { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
        public required Guid UserId { get; set; }

        // NEW: a list of the image URLs
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
