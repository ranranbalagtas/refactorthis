using RefactorThis.Domain.Entities;

namespace RefactorThis.Application.Dtos
{
    public class PaymentResponse
    {
        public string Message { get; set; }
        public Invoice Invoice { get; set; }
    }
}
