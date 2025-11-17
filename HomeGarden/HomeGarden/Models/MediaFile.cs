using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class MediaFile
{
    public long FileId { get; set; }

    public long? UserId { get; set; }

    public string? Entity { get; set; }

    public long? EntityId { get; set; }

    public string? FileUrl { get; set; }

    public string? FileName { get; set; }

    public string? MimeType { get; set; }

    public decimal? SizeKb { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual User? User { get; set; }
}
