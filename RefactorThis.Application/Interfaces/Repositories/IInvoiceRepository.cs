using RefactorThis.Domain.Entities;
using System.Threading.Tasks;

namespace RefactorThis.Application.Interfaces.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoice> GetInvoiceAsync(string reference);
        Task SaveAsync(Invoice invoice);
    }
}
