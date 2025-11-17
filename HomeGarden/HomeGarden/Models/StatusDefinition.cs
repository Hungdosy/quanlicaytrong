using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class StatusDefinition
{
    public int StatusId { get; set; }

    public string Entity { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();

    public virtual ICollection<Area> Areas { get; set; } = new List<Area>();

    public virtual ICollection<PlantLog> PlantLogs { get; set; } = new List<PlantLog>();

    public virtual ICollection<PlantResourceUsage> PlantResourceUsages { get; set; } = new List<PlantResourceUsage>();

    public virtual ICollection<Plant> Plants { get; set; } = new List<Plant>();

    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
