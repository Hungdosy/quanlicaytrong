using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class SubscriptionPayment
{
    public long PaymentId { get; set; }

    public long SubscriptionId { get; set; }

    public long UserId { get; set; }

    public int PlanId { get; set; }

    public decimal Amount { get; set; }

    public string? Currency { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentMethod { get; set; }

    public string? ProviderRef { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ProviderId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual SubscriptionPlan Plan { get; set; } = null!;

    public virtual PaymentProvider? Provider { get; set; }

    public virtual UserSubscription Subscription { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
