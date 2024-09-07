namespace RefactorThis.Application.Processors
{
    public class StandardInvoiceProcessor : BaseInvoiceProcessor
    {
        protected override decimal CalculateTax(decimal amount)
        {
            // Standard invoices do not have tax
            return 0;
        }
    }
}   
