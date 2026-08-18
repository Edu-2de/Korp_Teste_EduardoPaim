using Microsoft.EntityFrameworkCore;
using Billing.API.Infrastructure.Data;
using Billing.API.Domain.Models;

namespace Billing.API.Application.Services
{
    public class InvoiceService(BillingDbContext context) : IInvoiceService
    {
        public async Task<Invoice> CreateAsync()
        {
            var invoice = new Invoice();

            context.Invoices.Add(invoice);
            await context.SaveChangesAsync();

            return invoice;
        }

        public async Task<Invoice?> GetByIdAsync(Guid id)
        {
            return await context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await context.Invoices
                .Include(i => i.Items)
                .ToListAsync();
        }

        public async Task AddItemAsync(Guid invoiceId, Guid productId, int quantity)
        {
            var invoice = await context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == invoiceId)
                ?? throw new KeyNotFoundException($"Invoice {invoiceId} not found");

            var newItem = invoice.AddItem(productId, quantity);

            if (newItem != null)
                context.InvoiceItems.Add(newItem);

            await context.SaveChangesAsync();
        }
    }
}
