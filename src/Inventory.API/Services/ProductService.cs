using Microsoft.EntityFrameworkCore;
using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.DTOs;

namespace Inventory.API.Services
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

        public async Task<Product> CreateAsync(CreateProcutDto dto)
        {
            var product = new Product
            {
                Code = dto.Code,
                Balance = dto.Balance,
                Description = dto.Description

            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;

        }
    }
}
