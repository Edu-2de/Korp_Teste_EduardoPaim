using Microsoft.EntityFrameworkCore;
using Inventory.API.Infrastructure.Data;
using Inventory.API.Domain.Models;
using Inventory.API.Api.DTOs;

namespace Inventory.API.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly InventoryDbContext _context;

        public ProductService(InventoryDbContext context)
        {
            _context = context;
        }

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
            var codeExists = await _context.Products.AnyAsync(p => p.Code == dto.Code);
            if (codeExists)
            {
                throw new InvalidOperationException
                ($"Product with code '{dto.Code} already exists'");
            }

            var product = new Product(dto.Code, dto.Description, dto.Balance);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;

        }
    }
}
