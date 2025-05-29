using Domain.Types.Property;

namespace Domain.Entities
{
    public class Property
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required PropertyType Type { get; set; }
        public required PropertyStatus Status { get; set; }
        public required decimal Price { get; set; }
        public required string Address { get; set; }
        public required decimal Area { get; set; }
        public required int Rooms { get; set; }
        public required int Bathrooms { get; set; }
        public required int ConstructionYear { get; set; }
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        // Foreign key
        public required Guid UserId { get; set; }
        // Navigation property
        public required UserInformation User { get; set; }
        // New: one-to-many relationship
        public ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();
    }
}
