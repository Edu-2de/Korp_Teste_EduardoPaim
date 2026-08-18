using Billing.API.Domain.Models;
using Xunit;

namespace Billing.API.Tests.Domain
{
    public class DomainAddItemTests
    {
        [Fact]
        public void AddItem_ShouldAddNewItem_WhenProductIsNotYetInInvoice()
        {
            var invoice = new Invoice();
            var productId = Guid.NewGuid();

            invoice.AddItem(productId, 2);

            Assert.Single(invoice.Items);
            Assert.Equal(2, invoice.Items.First().Quantity);
        }

        [Fact]
        public void AddItem_ShouldConsolidateQuantity_WhenSameProductIsAddedTwice()
        {
            var invoice = new Invoice();
            var productId = Guid.NewGuid();

            invoice.AddItem(productId, 2);
            invoice.AddItem(productId, 3);

            Assert.Single(invoice.Items);
            Assert.Equal(5, invoice.Items.First().Quantity);
        }

        [Fact]
        public void AddItem_ShouldAddSeparateItems_WhenDifferentProductsAreAdded()
        {
            var invoice = new Invoice();

            invoice.AddItem(Guid.NewGuid(), 2);
            invoice.AddItem(Guid.NewGuid(), 1);

            Assert.Equal(2, invoice.Items.Count);
        }

        [Fact]
        public void AddItem_ShouldThrow_WhenInvoiceIsClosed()
        {
            var invoice = new Invoice();
            invoice.AddItem(Guid.NewGuid(), 1);
            invoice.Close();

            Assert.Throws<InvalidOperationException>(() => invoice.AddItem(Guid.NewGuid(), 1));
        }
    }
}
