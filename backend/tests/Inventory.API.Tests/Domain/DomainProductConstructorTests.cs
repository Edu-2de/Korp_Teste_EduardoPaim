using Inventory.API.Domain.Models;
using Xunit;

namespace Inventory.API.Tests.Domain
{
    public class DomainProductConstructorTests
    {
        [Fact]
        public void Constructor_ShouldThrow_WhenBalanceIsNegative()
        {
            Assert.Throws<ArgumentException>(() => new Product("P001", "Test", -1));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenCodeIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => new Product("", "Test", 10));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenDescriptionIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => new Product("P001", "", 10));
        }

        [Fact]
        public void Constructor_ShouldCreateProduct_WhenValid()
        {
            var product = new Product("P001", "Test", 10);

            Assert.NotEqual(Guid.Empty, product.Id);
            Assert.Equal("P001", product.Code);
            Assert.Equal("Test", product.Description);
            Assert.Equal(10, product.Balance);
        }
    }
}
