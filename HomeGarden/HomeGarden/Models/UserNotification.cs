using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class UserNotification
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string? Channel { get; set; }

    public string? Type { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual User User { get; set; } = null!;
}
