using Inventory.API.Domain.Models;
using Inventory.API.Api.DTOs;


namespace Inventory.API.Application.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product> CreateAsync(CreateProductDto dto);
        Task DecreaseBalanceAsync(Guid productId, int quantity, string idempotencyKey);

        Task UpdateDescriptionAsync(Guid productId, string newDescription);
        Task UpdateBalanceAsync(Guid productId, int newBalance);
        Task DeactivateAsync(Guid productId);

    }
}
