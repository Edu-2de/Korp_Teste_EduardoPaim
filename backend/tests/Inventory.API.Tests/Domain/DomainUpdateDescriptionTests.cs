using Inventory.API.Domain.Models;
using Xunit;

namespace Inventory.API.Tests.Domain
{
    public class DomainUpdateDescriptionTests
    {
        [Fact]
        public void UpdateDescription_ShouldChangeDescription_WhenValid()
        {
            var product = new Product("P001", "Old Description", 5);

            product.UpdateDescription("New Description");

            Assert.Equal("New Description", product.Description);
        }

        [Fact]
        public void UpdateDescription_ShouldThrow_WhenEmpty()
        {
            var product = new Product("P001", "Old Description", 5);

            Assert.Throws<ArgumentException>(() => product.UpdateDescription(""));
        }
    }
}
