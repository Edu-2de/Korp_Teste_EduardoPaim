using Inventory.API.Domain.Models;
using Xunit;

namespace Inventory.API.Tests.Domain
{
    public class DomainIncreaseBalanceTests
    {
        [Fact]
        public void IncreaseBalance_ShouldAddToBalance()
        {
            var product = new Product("P001", "Test", 5);

            product.IncreaseBalance(3);

            Assert.Equal(8, product.Balance);
        }

        [Fact]
        public void IncreaseBalance_ShouldThrow_WhenQuantityIsZeroOrNegative()
        {
            var product = new Product("P001", "Test", 5);

            Assert.Throws<ArgumentException>(() => product.IncreaseBalance(0));
            Assert.Throws<ArgumentException>(() => product.IncreaseBalance(-3));
        }
    }
}
