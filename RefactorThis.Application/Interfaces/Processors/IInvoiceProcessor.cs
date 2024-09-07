using RefactorThis.Application.Dtos;
using RefactorThis.Domain.Entities;
using System.Threading.Tasks;

namespace RefactorThis.Application.Interfaces.Processors
{
    public interface IInvoiceProcessor
    {
        Task<PaymentResponse> ProcessPaymentAsync(Invoice invoice, Payment payment);
    }
}
