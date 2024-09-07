using RefactorThis.Domain.Entities;
using System.Data.Entity;

namespace RefactorThis.Application.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }
}
