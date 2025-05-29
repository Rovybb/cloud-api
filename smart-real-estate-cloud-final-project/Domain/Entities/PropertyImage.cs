namespace Domain.Entities
{
    public class PropertyImage
    {
        public Guid Id { get; set; }

        // References the Property
        public Guid PropertyId { get; set; }
        public Property Property { get; set; }

        // If storing images in the cloud:
        public required string Url { get; set; }

        // If storing images in the DB:
        // public byte[] Data { get; set; }
        // public string ContentType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
