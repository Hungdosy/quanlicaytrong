using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class EmailTemplate
{
    public int TemplateId { get; set; }

    public string? Code { get; set; }

    public string? Subject { get; set; }

    public string? BodyHtml { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
