using Inventory.API.Api.DTOs;
using Inventory.API.Domain.Models;
using Inventory.API.Application.Services;
using Microsoft.AspNetCore.Mvc;


namespace Inventory.API.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IProductService _productService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            return Ok(await _productService.GetAllAsync());
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Product {id} not found.");

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create(CreateProductDto dto)
        {
            var product = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPatch("{id}/decrease-balance")]
        public async Task<IActionResult> DecreaseBalance(Guid id, DecreaseBalanceDto dto)
        {
            await _productService.DecreaseBalanceAsync(id, dto.Quantity);
            return NoContent();
        }
    }
}
