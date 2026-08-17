using Microsoft.EntityFrameworkCore;
using Inventory.API.Infrastructure.Data;
using Inventory.API.Application.Services;
using Inventory.API.Domain.Models;
using Inventory.API.Api.DTOs;
using Xunit;

namespace Inventory.API.Tests.Services
{
    public class ProductServiceTests
    {
        private InventoryDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new InventoryDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldPersistProduct()
        {
            using var context = CreateInMemoryContext(nameof(CreateAsync_ShouldPersistProduct));
            var service = new ProductService(context);
            var dto = new CreateProductDto { Code = "P001", Description = "Test Product", Balance = 10 };

            var result = await service.CreateAsync(dto);

            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("P001", result.Code);
            Assert.Equal(10, result.Balance);
            Assert.Equal(1, await context.Products.CountAsync());
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenCodeAlreadyExists()
        {
            using var context = CreateInMemoryContext(nameof(CreateAsync_ShouldThrow_WhenCodeAlreadyExists));
            var service = new ProductService(context);
            await service.CreateAsync(new CreateProductDto { Code = "P001", Description = "First", Balance = 5 });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateAsync(new CreateProductDto { Code = "P001", Description = "Duplicate", Balance = 3 }));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            using var context = CreateInMemoryContext(nameof(GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist));
            var service = new ProductService(context);

            var result = await service.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            using var context = CreateInMemoryContext(nameof(GetAllAsync_ShouldReturnAllProducts));
            var service = new ProductService(context);

            await service.CreateAsync(new CreateProductDto { Code = "P001", Description = "A", Balance = 5 });
            await service.CreateAsync(new CreateProductDto { Code = "P002", Description = "B", Balance = 3 });

            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count());
        }
    }

    public class ProductTests
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

        [Fact]
        public void IncreaseBalance_ShouldAddToBalance()
        {
            var product = new Product("P001", "Test", 5);

            product.IncreaseBalance(3);

            Assert.Equal(8, product.Balance);
        }
    }
}
