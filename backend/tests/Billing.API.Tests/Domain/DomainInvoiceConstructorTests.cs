using Billing.API.Domain.Models;
using Xunit;

namespace Billing.API.Tests.Domain
{
    public class DomainInvoiceConstructorTests
    {
        [Fact]
        public void Constructor_ShouldCreateInvoice_WithOpenStatusAndNoItems()
        {
            var invoice = new Invoice();

            Assert.NotEqual(Guid.Empty, invoice.Id);
            Assert.Equal(InvoiceStatus.Open, invoice.Status);
            Assert.Empty(invoice.Items);
        }

        [Fact]
        public void Constructor_ShouldSetCreatedAt_ToUtcNow()
        {
            var before = DateTime.UtcNow;
            var invoice = new Invoice();
            var after = DateTime.UtcNow;

            Assert.InRange(invoice.CreatedAt, before, after);
        }
    }
}
