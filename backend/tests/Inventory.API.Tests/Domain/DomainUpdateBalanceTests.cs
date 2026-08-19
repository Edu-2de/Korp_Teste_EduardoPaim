using Inventory.API.Domain.Models;
using Xunit;

namespace Inventory.API.Tests.Domain
{
    public class DomainUpdateBalanceTests
    {
        [Fact]
        public void UpdateBalance_ShouldSetBalance_WhenValid()
        {
            var product = new Product("P001", "Test", 5);

            product.UpdateBalance(42);

            Assert.Equal(42, product.Balance);
        }

        [Fact]
        public void UpdateBalance_ShouldAllowZero()
        {
            var product = new Product("P001", "Test", 5);

            product.UpdateBalance(0);

            Assert.Equal(0, product.Balance);
        }

        [Fact]
        public void UpdateBalance_ShouldThrow_WhenNegative()
        {
            var product = new Product("P001", "Test", 5);

            Assert.Throws<ArgumentException>(() => product.UpdateBalance(-1));
        }
    }
}
