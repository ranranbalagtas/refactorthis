using Moq;
using NUnit.Framework;
using RefactorThis.Application.Interfaces.Processors;
using RefactorThis.Application.Interfaces.Repositories;
using RefactorThis.Application.Interfaces.Services;
using RefactorThis.Application.Processors;
using RefactorThis.Application.Services;
using RefactorThis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefactorThis.Infrastructure.Tests
{
    [TestFixture]
    public class InvoiceServiceTests
    {
        private readonly IInvoiceService _invoiceService;
        private readonly Mock<IInvoiceRepository> _mockInvoiceRepository;
        private readonly IInvoiceProcessor _mockStandardInvoiceProcessor;
        private readonly IInvoiceProcessor _mockCommercialInvoiceProcessor;

        public InvoiceServiceTests()
        {
            _mockInvoiceRepository = new Mock<IInvoiceRepository>();
            _mockStandardInvoiceProcessor = new StandardInvoiceProcessor();
            _mockCommercialInvoiceProcessor = new CommercialInvoiceProcessor();

            _invoiceService = new InvoiceService(
                _mockInvoiceRepository.Object,
                _mockStandardInvoiceProcessor,
                _mockCommercialInvoiceProcessor);
        }

        [Test]
        public async Task ProcessPaymentAsync_Should_ThrowException_When_NoInvoiceFoundForPaymentReferenceAsync()
        {
            // Arrange
            var payment = new Payment();
            var expectedResult = "There is no invoice matching this payment.";

            _mockInvoiceRepository
                .Setup(repo => repo.GetInvoiceAsync(It.IsAny<string>()))
                .ReturnsAsync((Invoice)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => _invoiceService.ProcessPaymentAsync(payment));

            Assert.AreEqual(expectedResult, exception.Message);
        }

        [Test]
        public async Task ProcessPaymentAsync_Should_ReturnFailureMessage_When_NoPaymentNeededAsync()
        {
            // Arrange
            var payment = new Payment();
            var invoice = new Invoice()
            {
                Amount = 0,
                AmountPaid = 0,
                Payments = null
            };
            var expectedResult = "No payment needed.";

            _mockInvoiceRepository
                .Setup(repo => repo.GetInvoiceAsync(It.IsAny<string>()))
                .ReturnsAsync(invoice);

            // Act
            var result = await _invoiceService.ProcessPaymentAsync(payment);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task ProcessPaymentAsync_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaidAsync()
        {
            // Arrange
            var payment = new Payment();
            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 10,
                Payments = new List<Payment>()
                {
                    new Payment
                    {
                        Amount = 10
                    }
                }
            };
            var expectedResult = "Invoice was already fully paid.";

            _mockInvoiceRepository
                .Setup(repo => repo.GetInvoiceAsync(It.IsAny<string>()))
                .ReturnsAsync(invoice);

            // Act
            var result = await _invoiceService.ProcessPaymentAsync(payment);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task ProcessPaymentAsync_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDueAsync()
        {
            // Arrange
            var payment = new Payment()
            {
                Amount = 6
            };
            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>()
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };
            var expectedResult = "The payment is greater than the partial amount remaining.";

            _mockInvoiceRepository
                .Setup(repo => repo.GetInvoiceAsync(It.IsAny<string>()))
                .ReturnsAsync(invoice);

            // Act
            var result = await _invoiceService.ProcessPaymentAsync(payment);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task ProcessPaymentAsync_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmountAsync()
        {
            // Arrange
            var payment = new Payment()
            {
                Amount = 6
            };
            var invoice = new Invoice()
            {
                Amount = 5,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };  
            var expectedResult = "The payment is greater than the invoice amount.";

            _mockInvoiceRepository
                .Setup(repo => repo.GetInvoiceAsync(It.IsAny<string>()))
                .ReturnsAsync(invoice);

            // Act
            var result = await _invoiceService.ProcessPaymentAsync(payment);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task ProcessPaymentAsync_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDueAsync()
        {
            // Arrange
            var payment = new Payment()
            {
                Amount = 5
            };
            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>()
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };
            var expectedResult = "Final partial payment received, invoice is now fully paid.";

            _mockInvoiceRepository
                .Setup(repo => repo.GetInvoiceAsync(It.IsAny<string>()))
                .ReturnsAsync(invoice);

            // Act
            var result = await _invoiceService.ProcessPaymentAsync(payment);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task ProcessPaymentAsync_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmountAsync()
        {
            // Arrange
            var payment = new Payment()
            {
                Amount = 10
            };
            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>()
                {
                    new Payment
                    {
                        Amount = 10
                    }
                }
            };
            var expectedResult = "Invoice was already fully paid.";

            _mockInvoiceRepository
                .Setup(repo => repo.GetInvoiceAsync(It.IsAny<string>()))
                .ReturnsAsync(invoice);

            // Act
            var result = await _invoiceService.ProcessPaymentAsync(payment);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task ProcessPaymentAsync_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDueAsync()
        {
            // Arrange
            var payment = new Payment()
            {
                Amount = 1
            };
            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment>()
                {
                    new Payment
                    {
                        Amount = 5
                    }
                }
            };
            var expectedResult = "Partial payment received, still not fully paid.";

            _mockInvoiceRepository
                .Setup(repo => repo.GetInvoiceAsync(It.IsAny<string>()))
                .ReturnsAsync(invoice);

            // Act
            var result = await _invoiceService.ProcessPaymentAsync(payment);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task ProcessPaymentAsync_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmountAsync()
        {
            // Arrange
            var payment = new Payment()
            {
                Amount = 1
            };
            var invoice = new Invoice()
            {
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            var expectedResult = "Partial payment received, still not fully paid.";

            _mockInvoiceRepository
                .Setup(repo => repo.GetInvoiceAsync(It.IsAny<string>()))
                .ReturnsAsync(invoice);

            // Act
            var result = await _invoiceService.ProcessPaymentAsync(payment);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}   
