using Inventory.API.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Infrastructure.Data
{
    public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
    }
}
