using Microsoft.EntityFrameworkCore;
using Inventory.API.Infrastructure.Data;
using Inventory.API.Domain.Models;
using Inventory.API.Api.DTOs;

namespace Inventory.API.Application.Services
{
    public class ProductService(InventoryDbContext context) : IProductService
    {
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await context.Products.FindAsync(id);
        }

        public async Task<Product> CreateAsync(CreateProductDto dto)
        {
            var code = string.IsNullOrWhiteSpace(dto.Code)
                ? GenerateProductCode()
                : dto.Code;

            var codeExists = await context.Products.AnyAsync(p => p.Code == code);
            if (codeExists)
            {
                throw new InvalidOperationException($"Product with code '{code}' already exists");
            }

            var product = new Product(code, dto.Description, dto.Balance);

            context.Products.Add(product);
            await context.SaveChangesAsync();

            return product;
        }

        public async Task DecreaseBalanceAsync(Guid productId, int quantity)
        {
            var product = await context.Products.FindAsync(productId)
                ?? throw new KeyNotFoundException($"Product {productId} not found");

            product.DecreaseBalance(quantity);

            await context.SaveChangesAsync();
        }

        public async Task UpdateDescriptionAsync(Guid productId, string newDescription)
        {
            var product = await context.Products.FindAsync(productId)
                ?? throw new KeyNotFoundException($"Product {productId} not found");

            product.UpdateDescription(newDescription);

            await context.SaveChangesAsync();
        }

        private static string GenerateProductCode()
        {
            return $"PROD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }
    }
}
