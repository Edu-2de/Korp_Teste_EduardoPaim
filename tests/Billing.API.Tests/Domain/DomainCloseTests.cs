using Billing.API.Domain.Models;
using Xunit;

namespace Billing.API.Tests.Domain
{
    public class DomainCloseTests
    {
        [Fact]
        public void Close_ShouldSetStatusToClosed_WhenInvoiceHasItems()
        {
            var invoice = new Invoice();
            invoice.AddItem(Guid.NewGuid(), 1);

            invoice.Close();

            Assert.Equal(InvoiceStatus.Closed, invoice.Status);
        }

        [Fact]
        public void Close_ShouldThrow_WhenInvoiceHasNoItems()
        {
            var invoice = new Invoice();

            Assert.Throws<InvalidOperationException>(() => invoice.Close());
        }

        [Fact]
        public void Close_ShouldThrow_WhenInvoiceIsAlreadyClosed()
        {
            var invoice = new Invoice();
            invoice.AddItem(Guid.NewGuid(), 1);
            invoice.Close();

            Assert.Throws<InvalidOperationException>(() => invoice.Close());
        }
    }
}
