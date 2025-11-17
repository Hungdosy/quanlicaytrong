using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class ResourceType
{
    public int TypeId { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
