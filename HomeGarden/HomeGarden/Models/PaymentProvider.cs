using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class PaymentProvider
{
    public int ProviderId { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? ApiKey { get; set; }

    public string? SecretKey { get; set; }

    public string? EndpointUrl { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<SubscriptionPayment> SubscriptionPayments { get; set; } = new List<SubscriptionPayment>();
}
