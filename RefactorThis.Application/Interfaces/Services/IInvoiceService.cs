using RefactorThis.Domain.Entities;
using System.Threading.Tasks;

namespace RefactorThis.Application.Interfaces.Services
{
    public interface IInvoiceService
    {
        Task<string> ProcessPaymentAsync(Payment payment);
    }
}
