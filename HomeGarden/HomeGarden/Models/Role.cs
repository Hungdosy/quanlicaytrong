using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public int? StatusId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual StatusDefinition? Status { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
