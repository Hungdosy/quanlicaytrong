namespace HomeGarden.Dtos
{
    public class PlantLogDto
    {
        public long LogId { get; set; }
        public long PlantId { get; set; }
        public string Activity { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Health { get; set; }
        public DateTime LogDate { get; set; }
    }

    public class PlantLogCreateDto
    {
        public long PlantId { get; set; }
        public string Activity { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int? HealthId { get; set; }
    }
}
