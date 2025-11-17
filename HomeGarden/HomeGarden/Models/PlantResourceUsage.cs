using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class PlantResourceUsage
{
    public long UsageId { get; set; }

    public long PlantId { get; set; }

    public long ResourceId { get; set; }

    public DateTime? UsedAt { get; set; }

    public decimal? QuantityUsed { get; set; }

    public string? Note { get; set; }

    public int StatusId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Plant Plant { get; set; } = null!;

    public virtual Resource Resource { get; set; } = null!;

    public virtual StatusDefinition Status { get; set; } = null!;
}
