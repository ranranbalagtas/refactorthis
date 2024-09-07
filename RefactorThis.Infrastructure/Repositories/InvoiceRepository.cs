using RefactorThis.Application.Data;
using RefactorThis.Application.Interfaces.Repositories;
using RefactorThis.Domain.Entities;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;

namespace RefactorThis.Application.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _context;

        public InvoiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice> GetInvoiceAsync(string reference)
        {
            var context = new AppDbContext();

            var payment = await _context.Payments
                .Include("Invoice")
                .FirstOrDefaultAsync(p => p.Reference == reference);

            if (payment == null)
            {
                throw new InvalidOperationException("Payment not found.");
            }

            return payment.Invoice;
        }

        public async Task SaveAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);

            await _context.SaveChangesAsync();
        }
    }
}
