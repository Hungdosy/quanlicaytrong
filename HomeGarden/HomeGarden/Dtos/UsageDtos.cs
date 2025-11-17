namespace HomeGarden.Dtos
{
    public class UsageDto
    {
        public long UsageId { get; set; }
        public long PlantId { get; set; }
        public long ResourceId { get; set; }
        public string ResourceName { get; set; } = string.Empty;
        public decimal QuantityUsed { get; set; }
        public string? Note { get; set; }
        public DateTime UsedAt { get; set; }
    }

    public class UsageCreateDto
    {
        public long PlantId { get; set; }
        public long ResourceId { get; set; }
        public decimal QuantityUsed { get; set; }
        public string? Note { get; set; }
        public int StatusId { get; set; }
    }
}
