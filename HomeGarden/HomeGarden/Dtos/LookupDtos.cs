namespace HomeGarden.Dtos
{
    public class StatusDto
    {
        public int StatusId { get; set; }
        public string Entity { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class HealthDto
    {
        public int HealthId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class ResourceTypeDto
    {
        public int TypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
