using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class HealthDefinition
{
    public int HealthId { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<PlantLog> PlantLogs { get; set; } = new List<PlantLog>();

    public virtual ICollection<Plant> Plants { get; set; } = new List<Plant>();
}
