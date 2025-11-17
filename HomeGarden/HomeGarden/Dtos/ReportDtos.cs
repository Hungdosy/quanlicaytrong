namespace HomeGarden.Dtos
{
    public class SystemReportDto
    {
        public int TotalUsers { get; set; }
        public int TotalPlants { get; set; }
        public int DeadPlants { get; set; }
        public int HarvestedPlants { get; set; }
        public int OverdueSchedules { get; set; }
        public int ActiveAlerts { get; set; }
        public int LowResources { get; set; }
    }

    public class PlantReportDto
    {
        public long PlantId { get; set; }
        public string PlantName { get; set; } = string.Empty;
        public int TotalLogs { get; set; }
        public int TotalAlerts { get; set; }
        public int TotalSchedules { get; set; }
        public decimal TotalCost { get; set; }
    }
}
