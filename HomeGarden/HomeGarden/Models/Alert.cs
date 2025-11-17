using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class Alert
{
    public long AlertId { get; set; }

    public long PlantId { get; set; }

    public string AlertType { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime? AlertDate { get; set; }

    public bool? Resolved { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public int StatusId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Plant Plant { get; set; } = null!;

    public virtual StatusDefinition Status { get; set; } = null!;
}
