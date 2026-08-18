using Billing.API.Domain.Models;

namespace Billing.API.Application.Services
{
    public interface IInvoiceService
    {
        Task<Invoice> CreateAsync();
        Task<Invoice?> GetByIdAsync(Guid id);
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task AddItemAsync(Guid invoiceId, Guid productId, int quantity);
    }
}
