using RefactorThis.Application.Interfaces.Processors;
using RefactorThis.Domain.Entities;
using System.Linq;
using System;
using System.Threading.Tasks;
using RefactorThis.Application.Dtos;

namespace RefactorThis.Application.Processors
{
    public abstract class BaseInvoiceProcessor : IInvoiceProcessor
    {
        public async Task<PaymentResponse> ProcessPaymentAsync(Invoice invoice, Payment payment)
        {
            if (invoice.Amount == 0)
            {
                return HandleZeroAmountInvoice(invoice);
            }

            if (invoice.Payments != null && invoice.Payments.Any())
            {
                return HandlePayments(invoice, payment);
            }

            return HandleNoPayments(invoice, payment);
        }

        private PaymentResponse HandleZeroAmountInvoice(Invoice invoice)
        {
            if (invoice.Payments == null || !invoice.Payments.Any())
            {
                return new PaymentResponse { Message = "No payment needed." };
            }

            throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
        }

        private PaymentResponse HandlePayments(Invoice invoice, Payment payment)
        {
            var totalPayments = invoice.Payments.Sum(x => x.Amount);

            if (totalPayments != 0 && totalPayments == invoice.Amount)
            {
                return new PaymentResponse { Message = "Invoice was already fully paid." };
            }

            if (totalPayments != 0 && payment.Amount > (invoice.Amount - invoice.AmountPaid))
            {
                return new PaymentResponse { Message = "The payment is greater than the partial amount remaining." };
            }

            return ProcessPartialPayment(invoice, payment);
        }

        private PaymentResponse HandleNoPayments(Invoice invoice, Payment payment)
        {
            if (payment.Amount > invoice.Amount)
            {
                return new PaymentResponse { Message = "The payment is greater than the invoice amount." };
            }

            if (invoice.Amount == payment.Amount)
            {
                return ProcessFullPayment(invoice, payment);
            }

            return ProcessPartialPayment(invoice, payment);
        }

        private PaymentResponse ProcessPartialPayment(Invoice invoice, Payment payment)
        {
            var response = new PaymentResponse();

            invoice.AmountPaid += payment.Amount;
            invoice.TaxAmount += CalculateTax(payment.Amount);
            invoice.Payments.Add(payment);

            response.Invoice = invoice;
            response.Message = (invoice.Amount == invoice.AmountPaid)
                ? "Final partial payment received, invoice is now fully paid."
                : "Partial payment received, still not fully paid.";

            return response;
        }

        private PaymentResponse ProcessFullPayment(Invoice invoice, Payment payment)
        {
            var response = new PaymentResponse();

            invoice.AmountPaid = payment.Amount;
            invoice.TaxAmount = CalculateTax(payment.Amount);
            invoice.Payments.Add(payment);

            response.Invoice = invoice;
            response.Message = "Invoice is now fully paid.";

            return response;
        }

        protected abstract decimal CalculateTax(decimal amount);
    }
}
