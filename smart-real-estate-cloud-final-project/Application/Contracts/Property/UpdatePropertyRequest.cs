namespace Application.Contracts.Property
{
    public class UpdatePropertyRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        //public PropertyType Type { get; set; }
        public decimal Price { get; set; }
        //public PropertyStatus Status { get; set; }
        public string Address { get; set; } = string.Empty;
        public decimal Area { get; set; }
        public int Rooms { get; set; }
        public int Bathrooms { get; set; }
        public int ConstructionYear { get; set; }
        public Guid UserId { get; set; }
    }
}
