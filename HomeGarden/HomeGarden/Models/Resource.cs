using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class Resource
{
    public long ResourceId { get; set; }

    public long UserId { get; set; }

    public string Name { get; set; } = null!;

    public int TypeId { get; set; }

    public decimal? Quantity { get; set; }

    public string? Unit { get; set; }

    public decimal? Cost { get; set; }

    public string? Note { get; set; }

    public int StatusId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<PlantResourceUsage> PlantResourceUsages { get; set; } = new List<PlantResourceUsage>();

    public virtual StatusDefinition Status { get; set; } = null!;

    public virtual ResourceType Type { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
