using RefactorThis.Domain.Enum;
using System.Collections.Generic;

namespace RefactorThis.Domain.Entities
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TaxAmount { get; set; }
        public InvoiceType Type { get; set; }

        // Navigation Property
        public IList<Payment> Payments { get; set; }
    }
}
