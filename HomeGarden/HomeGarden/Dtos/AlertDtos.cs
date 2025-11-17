namespace HomeGarden.Dtos
{
    public class AlertDto
    {
        public long AlertId { get; set; }
        public long PlantId { get; set; }
        public string AlertType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? AlertDate { get; set; }
        public bool Resolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }

    public class AlertCreateDto
    {
        public long PlantId { get; set; }
        public string AlertType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
