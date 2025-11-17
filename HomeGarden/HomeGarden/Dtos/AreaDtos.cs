namespace HomeGarden.Dtos
{
    public class AreaDto
    {
        public long AreaId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class AreaCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Description { get; set; }
    }

    public class AreaUpdateDto
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public int? StatusId { get; set; }
    }

    public class AreaPlantDto
    {
        public long PlantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string? Health { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? PlantedDate { get; set; }  // DateTime? để khớp DB
    }

}
