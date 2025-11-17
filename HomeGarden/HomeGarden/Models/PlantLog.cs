using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class PlantLog
{
    public long LogId { get; set; }

    public long PlantId { get; set; }

    public DateTime? LogDate { get; set; }

    public string Activity { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int? HealthId { get; set; }

    public int StatusId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual HealthDefinition? Health { get; set; }

    public virtual Plant Plant { get; set; } = null!;

    public virtual StatusDefinition Status { get; set; } = null!;
}
