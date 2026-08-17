using Inventory.API.Models;
using Inventory.API.DTOs;


namespace Inventory.API.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product> CreateAsync(CreateProcutDto dto);

    }
}
