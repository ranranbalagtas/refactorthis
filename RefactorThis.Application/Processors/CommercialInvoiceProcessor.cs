namespace RefactorThis.Application.Processors
{
    public class CommercialInvoiceProcessor : BaseInvoiceProcessor
    {
        protected override decimal CalculateTax(decimal amount)
        {
            // Commercial invoices have a 14% tax
            return amount * 0.14m;
        }
    }
}
