namespace HomeGarden.Dtos
{
    public class ResourceDto
    {
        public long ResourceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal? Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal? Cost { get; set; }
        public string? Note { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ResourceCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
        public decimal? Cost { get; set; }
        public string? Note { get; set; }
        public int StatusId { get; set; }
    }

    public class ResourceUpdateDto
    {
        public decimal? Quantity { get; set; }
        public string? Unit { get; set; }
        public decimal? Cost { get; set; }
        public string? Note { get; set; }
        public int? StatusId { get; set; }
    }
}
