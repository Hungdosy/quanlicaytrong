using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class Schedule
{
    public long ScheduleId { get; set; }

    public long PlantId { get; set; }

    public string TaskType { get; set; } = null!;

    public string Frequency { get; set; } = null!;

    public DateTime NextDue { get; set; }

    public DateTime? LastDone { get; set; }

    public bool? Reminder { get; set; }

    public int StatusId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Plant Plant { get; set; } = null!;

    public virtual StatusDefinition Status { get; set; } = null!;
}
