using Microsoft.EntityFrameworkCore;
using Billing.API.Domain.Models;

namespace Billing.API.Infrastructure.Data
{
    public class BillingDbContext(DbContextOptions<BillingDbContext> options) : DbContext(options)
    {

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.Property(i => i.Number)
                    .ValueGeneratedOnAdd();

                entity.HasMany(i => i.Items)
                    .WithOne()
                    .HasForeignKey(item => item.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Metadata
                    .FindNavigation(nameof(Invoice.Items))!
                    .SetPropertyAccessMode(PropertyAccessMode.Field);

            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.HasKey(item => item.Id);
                entity.Property(item => item.Id).ValueGeneratedNever();
            });
        }
    }
}
