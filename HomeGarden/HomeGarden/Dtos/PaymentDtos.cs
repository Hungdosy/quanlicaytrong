namespace HomeGarden.Dtos
{
    public class SubscriptionPaymentDto
    {
        public long PaymentId { get; set; }
        public string PlanName { get; set; }
        public string? UserEmail { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string? ProviderName { get; set; }
        public DateTime? PaymentDate { get; set; }
    }

    public class PaymentCreateDto
    {
        public long SubscriptionId { get; set; }
        public int ProviderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class PaymentStatusUpdateDto
    {
        public string Status { get; set; }   // Pending / Paid / Failed
        public string? ProviderRef { get; set; }
    }
}
