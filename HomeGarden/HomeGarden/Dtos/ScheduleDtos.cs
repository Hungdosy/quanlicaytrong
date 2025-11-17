namespace HomeGarden.Dtos
{
    public class ScheduleListDto
    {
        public long ScheduleId { get; set; }
        public long PlantId { get; set; }
        public string TaskType { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public DateTime NextDue { get; set; }
        public DateTime? LastDone { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ScheduleCreateDto
    {
        public long PlantId { get; set; }
        public string TaskType { get; set; } = string.Empty;
        public string Frequency { get; set; } = "daily";
        public DateTime NextDue { get; set; }
        public int Count { get; set; } = 1; 
    }


    public class ScheduleDoneDto
    {
        public DateTime DoneAt { get; set; } = DateTime.Now;
    }
}
