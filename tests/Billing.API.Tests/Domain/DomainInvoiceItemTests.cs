using Billing.API.Domain.Models;
using Xunit;

namespace Billing.API.Tests.Domain
{
    public class DomainInvoiceItemTests
    {
        [Fact]
        public void Constructor_ShouldThrow_WhenProductIdIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => new InvoiceItem(Guid.NewGuid(), Guid.Empty, 1));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenQuantityIsZeroOrNegative()
        {
            Assert.Throws<ArgumentException>(() => new InvoiceItem(Guid.NewGuid(), Guid.NewGuid(), 0));
            Assert.Throws<ArgumentException>(() => new InvoiceItem(Guid.NewGuid(), Guid.NewGuid(), -1));
        }

        [Fact]
        public void IncreaseQuantity_ShouldAddToQuantity_WhenAmountIsValid()
        {
            var item = new InvoiceItem(Guid.NewGuid(), Guid.NewGuid(), 2);

            item.IncreaseQuantity(3);

            Assert.Equal(5, item.Quantity);
        }

        [Fact]
        public void IncreaseQuantity_ShouldThrow_WhenAmountIsZeroOrNegative()
        {
            var item = new InvoiceItem(Guid.NewGuid(), Guid.NewGuid(), 2);

            Assert.Throws<ArgumentException>(() => item.IncreaseQuantity(0));
            Assert.Throws<ArgumentException>(() => item.IncreaseQuantity(-1));
        }
    }
}
