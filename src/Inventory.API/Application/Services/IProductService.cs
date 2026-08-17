using Inventory.API.Domain.Models;
using Inventory.API.Api.DTOs;


namespace Inventory.API.Application.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product> CreateAsync(CreateProductDto dto);

    }
}
