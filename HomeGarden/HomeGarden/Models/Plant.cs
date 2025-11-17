using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class Plant
{
    public long PlantId { get; set; }

    public long AreaId { get; set; }

    public string Name { get; set; } = null!;

    public string? Species { get; set; }

    public DateOnly? PlantedDate { get; set; }

    public string? ImageUrl { get; set; }

    public int? HealthId { get; set; }

    public int StatusId { get; set; }

    public string? Notes { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();

    public virtual Area Area { get; set; } = null!;

    public virtual HealthDefinition? Health { get; set; }

    public virtual ICollection<PlantLog> PlantLogs { get; set; } = new List<PlantLog>();

    public virtual ICollection<PlantResourceUsage> PlantResourceUsages { get; set; } = new List<PlantResourceUsage>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual StatusDefinition Status { get; set; } = null!;
}
