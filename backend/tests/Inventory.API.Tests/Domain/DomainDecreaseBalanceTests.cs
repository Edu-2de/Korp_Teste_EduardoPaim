using Inventory.API.Domain.Models;
using Xunit;

namespace Inventory.API.Tests.Domain
{
    public class DomainDecreaseBalanceTests
    {
        [Fact]
        public void DecreaseBalance_ShouldReduceBalance_WhenQuantityIsValid()
        {
            var product = new Product("P001", "Test", 10);

            product.DecreaseBalance(3);

            Assert.Equal(7, product.Balance);
        }

        [Fact]
        public void DecreaseBalance_ShouldAllowExactBalance()
        {
            var product = new Product("P001", "Test", 1);

            product.DecreaseBalance(1);

            Assert.Equal(0, product.Balance);
        }

        [Fact]
        public void DecreaseBalance_ShouldThrow_WhenInsufficientBalance()
        {
            var product = new Product("P001", "Test", 1);

            Assert.Throws<InvalidOperationException>(() => product.DecreaseBalance(2));
        }

        [Fact]
        public void DecreaseBalance_ShouldThrow_WhenQuantityIsZeroOrNegative()
        {
            var product = new Product("P001", "Test", 10);

            Assert.Throws<ArgumentException>(() => product.DecreaseBalance(0));
            Assert.Throws<ArgumentException>(() => product.DecreaseBalance(-5));
        }
    }
}
