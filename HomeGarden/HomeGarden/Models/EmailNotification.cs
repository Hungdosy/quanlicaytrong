using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class EmailNotification
{
    public long EmailId { get; set; }

    public long UserId { get; set; }

    public string? Subject { get; set; }

    public string? Content { get; set; }

    public bool? Sent { get; set; }

    public DateTime? SendTime { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual User User { get; set; } = null!;
}
