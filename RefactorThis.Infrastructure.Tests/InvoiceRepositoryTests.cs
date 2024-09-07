using NUnit.Framework;
using RefactorThis.Application.Data;
using RefactorThis.Application.Interfaces.Repositories;
using RefactorThis.Application.Repositories;
using RefactorThis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[TestFixture]
public class InvoiceRepositoryTests
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly AppDbContext _context;

    public InvoiceRepositoryTests()
    {
        _context = new AppDbContext();
        _invoiceRepository = new InvoiceRepository(_context);
    }

    [Test]
    public async Task SaveAsync_NoPaymentInvoice()
    {
        // Arrange
        var invoice = new Invoice()
        {
            Amount = 0,
            AmountPaid = 0,
            Payments = null
        };

        // Act & Assert
        Assert.DoesNotThrowAsync(() => _invoiceRepository.SaveAsync(invoice));
    }

    [Test]
    public async Task SaveAsync_PaymentInvoice()
    {
        // Arrange
        var invoice = new Invoice()
        {
            Amount = 10,
            AmountPaid = 10,
            Payments = new List<Payment>
            {
                new Payment
                {
                    Amount = 10
                }
            }
        };

        // Act & Assert
        Assert.DoesNotThrowAsync(() => _invoiceRepository.SaveAsync(invoice));
    }

    [Test]
    public async Task GetInvoiceAsync_ValidReference_ReturnInvoiceAsync()
    {
        // Arrange
        var payment = new Payment()
        {
            Amount = 10,
            Reference = "valid-reference"
        };
        var invoice = new Invoice()
        {
            Amount = 10,
            AmountPaid = 10,
            Payments = new List<Payment>() { payment }
        };

        await _invoiceRepository.SaveAsync(invoice);

        // Act
        var result = await _invoiceRepository.GetInvoiceAsync("valid-reference");

        // Assert
        Assert.AreEqual(result.Amount, invoice.Amount);
        Assert.IsTrue(invoice.Payments.Contains(payment));
    }

    [Test]
    public async Task GetInvoiceAsync_InvalidReference_ReturnInvoiceAsync()
    {
        // Arrange
        var payment = new Payment()
        {
            Amount = 5,
            Reference = "another-valid-reference"
        };
        var invoice = new Invoice()
        {
            Amount = 5,
            AmountPaid = 5,
            Payments = new List<Payment>() { payment }
        };

        await _invoiceRepository.SaveAsync(invoice);

        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(() => _invoiceRepository.GetInvoiceAsync("invalid-reference"));

        Assert.AreEqual("Payment not found.", exception.Message);
    }
}