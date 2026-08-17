using Microsoft.EntityFrameworkCore;
using Inventory.API.Infrastructure.Data;
using Inventory.API.Domain.Models;
using Inventory.API.Api.DTOs;

namespace Inventory.API.Application.Services
{
    public class ProductService(InventoryDbContext _context) : IProductService
    {
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> CreateAsync(CreateProductDto dto)
        {

            var code = string.IsNullOrWhiteSpace(dto.Code)
                ? GenerateProductCode()
                : dto.Code;


            var codeExists = await _context.Products.AnyAsync(p => p.Code == dto.Code);
            if (codeExists)
            {
                throw new InvalidOperationException
                ($"Product with code '{dto.Code}' already exists");
            }

            var product = new Product(code, dto.Description, dto.Balance);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;

        }

        private static string GenerateProductCode()
        {
            return $"PROD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

        public async Task DecreaseBalanceAsync(Guid productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId)
                ?? throw new KeyNotFoundException($"Product {productId} not found");

            product.DecreaseBalance(quantity);

            await _context.SaveChangesAsync();




        }
    }
}
