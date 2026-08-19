using Billing.API.Domain.Models;
using Xunit;

namespace Billing.API.Tests.Domain
{
    public class DomainRemoveItemTests
    {
        [Fact]
        public void RemoveItem_ShouldRemoveIt_WhenInvoiceIsOpen()
        {
            var invoice = new Invoice();
            var item = invoice.AddItem(Guid.NewGuid(), 2)!;

            invoice.RemoveItem(item.Id);

            Assert.Empty(invoice.Items);
        }

        [Fact]
        public void RemoveItem_ShouldThrow_WhenItemNotFound()
        {
            var invoice = new Invoice();

            Assert.Throws<KeyNotFoundException>(() => invoice.RemoveItem(Guid.NewGuid()));
        }

        [Fact]
        public void RemoveItem_ShouldThrow_WhenInvoiceIsClosed()
        {
            var invoice = new Invoice();
            var item = invoice.AddItem(Guid.NewGuid(), 1)!;
            invoice.Close();

            Assert.Throws<InvalidOperationException>(() => invoice.RemoveItem(item.Id));
        }
    }
}
