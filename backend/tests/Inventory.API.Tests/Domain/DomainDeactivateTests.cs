using Inventory.API.Domain.Models;
using Xunit;

namespace Inventory.API.Tests.Domain
{
    public class DomainDeactivateTests
    {
        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse_WhenActive()
        {
            var product = new Product("P001", "Test", 5);

            product.Deactivate();

            Assert.False(product.IsActive);
        }

        [Fact]
        public void Deactivate_ShouldPreserveBalance()
        {
            var product = new Product("P001", "Test", 5);

            product.Deactivate();

            Assert.Equal(5, product.Balance);
        }

        [Fact]
        public void Deactivate_ShouldThrow_WhenAlreadyInactive()
        {
            var product = new Product("P001", "Test", 5);
            product.Deactivate();

            Assert.Throws<InvalidOperationException>(() => product.Deactivate());
        }
    }
}
