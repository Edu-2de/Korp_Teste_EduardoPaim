using Microsoft.EntityFrameworkCore;
using Inventory.API.Domain.Models;

namespace Inventory.API.Infrastructure.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property<uint>("xmin")
                .HasColumnName("xmin")
                .IsRowVersion();

            base.OnModelCreating(modelBuilder);
        }
    }
}
