using Microsoft.EntityFrameworkCore;
using Inventory.API.Domain.Models;

namespace Inventory.API.Infrastructure.Data
{
    public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property<uint>("xmin")
                .HasColumnName("xmin")
                .IsRowVersion();

            modelBuilder.Entity<IdempotencyRecord>(entity =>
            {
                entity.HasKey(r => r.Key);
            });
        }
    }
}
