namespace HomeGarden.Dtos
{
    public class PlantListDto
    {
        public long PlantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string? Health { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? PlantedDate { get; set; }
    }

    public class PlantDetailDto
    {
        public long PlantId { get; set; }
        public long AreaId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Species { get; set; }
        public DateTime? PlantedDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? Notes { get; set; }
        public string? Health { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class PlantCreateDto
    {
        public long AreaId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Species { get; set; }
        public DateTime? PlantedDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? Notes { get; set; }
        public int? HealthId { get; set; }
        public int StatusId { get; set; }
    }

    public class PlantUpdateDto
    {
        public string? Name { get; set; }
        public string? Species { get; set; }
        public DateTime? PlantedDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? Notes { get; set; }
        public int? HealthId { get; set; }
        public int? StatusId { get; set; }
    }
}
