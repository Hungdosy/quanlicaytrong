using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class Area
{
    public long AreaId { get; set; }

    public long UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Location { get; set; }

    public string? Description { get; set; }

    public int StatusId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Plant> Plants { get; set; } = new List<Plant>();

    public virtual StatusDefinition Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
