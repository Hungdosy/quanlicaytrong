using System;
using System.Collections.Generic;

namespace HomeGarden.Models;

public partial class UserSubscription
{
    public long SubscriptionId { get; set; }

    public long UserId { get; set; }

    public int PlanId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime NextBillingDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Status { get; set; }

    public string? PaymentMethod { get; set; }

    public long? LastPaymentId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual SubscriptionPlan Plan { get; set; } = null!;

    public virtual ICollection<SubscriptionPayment> SubscriptionPayments { get; set; } = new List<SubscriptionPayment>();

    public virtual User User { get; set; } = null!;
}
