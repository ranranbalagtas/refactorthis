using RefactorThis.Application.Interfaces.Processors;
using RefactorThis.Application.Interfaces.Repositories;
using RefactorThis.Application.Interfaces.Services;
using RefactorThis.Domain.Entities;
using RefactorThis.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefactorThis.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly Dictionary<InvoiceType, IInvoiceProcessor> _processors;

        public InvoiceService(IInvoiceRepository invoiceRepository, IInvoiceProcessor standardProcessor, IInvoiceProcessor commercialProcessor)
        {
            _invoiceRepository = invoiceRepository;
            _processors = new Dictionary<InvoiceType, IInvoiceProcessor>
            {
                { InvoiceType.Standard, standardProcessor },
                { InvoiceType.Commercial, commercialProcessor }
            };
        }

        public async Task<string> ProcessPaymentAsync(Payment payment)
        {
            var invoice = await _invoiceRepository.GetInvoiceAsync(payment.Reference);

            if (invoice == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment.");
            }

            if (_processors.TryGetValue(invoice.Type, out var processor))
            {
                var result = await processor.ProcessPaymentAsync(invoice, payment);

                await _invoiceRepository.SaveAsync(result.Invoice);
                
                return result.Message;
            }

            throw new ArgumentOutOfRangeException("Invalid invoice type.");
        }
    }
}
