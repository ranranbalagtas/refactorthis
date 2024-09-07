namespace RefactorThis.Domain.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }

        // Foreign Key
        public int InvoiceId { get; set; }

        // Navigation Property
        public Invoice Invoice { get; set; }
    }
}
