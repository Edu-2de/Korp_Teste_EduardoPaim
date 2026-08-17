using Inventory.API.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Infrastructure.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
