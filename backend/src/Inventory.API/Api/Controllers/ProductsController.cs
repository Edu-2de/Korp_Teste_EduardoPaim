using Microsoft.AspNetCore.Mvc;
using Inventory.API.Api.DTOs;
using Inventory.API.Application.Services;
using Inventory.API.Domain.Models;
using Shared.Kernel;

namespace Inventory.API.Api.Controllers
{
    /// <summary>
    /// Gerencia o cadastro de produtos e o controle de saldo em estoque.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Lista todos os produtos cadastrados.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products.Select(ToResponseDto));
        }

        /// <summary>
        /// Busca um produto pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador (GUID) do produto.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product {id} not found.");

            return Ok(ToResponseDto(product));
        }

        /// <summary>
        /// Cria um novo produto. Se o código não for informado, um código é gerado automaticamente
        /// no formato PROD-{timestamp}.
        /// </summary>
        /// <param name="dto">Dados do produto a ser criado.</param>
        [HttpPost]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ProductResponseDto>> Create(CreateProductDto dto)
        {
            var product = await _productService.CreateAsync(dto);
            var responseDto = ToResponseDto(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, responseDto);
        }

        /// <summary>
        /// Debita uma quantidade do saldo de um produto. Utilizado pelo Billing.API
        /// no momento da impressão de uma nota fiscal.
        /// </summary>
        /// <param name="id">Identificador (GUID) do produto.</param>
        /// <param name="dto">Quantidade a ser debitada.</param>
        /// <param name="idempotencyKey"></param>
        [HttpPatch("{id}/decrease-balance")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DecreaseBalance(
            Guid id,
            DecreaseBalanceDto dto,
            [FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey
        )
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey))
            {
                throw new ArgumentException("X-Idempotency-Key header is required.");
            }

            await _productService.DecreaseBalanceAsync(id, dto.Quantity, idempotencyKey);
            return NoContent();
        }

        /// <summary>
        /// Atualiza a descrição de um produto existente.
        /// </summary>
        /// <param name="id">Identificador (GUID) do produto.</param>
        /// <param name="dto">Nova descrição do produto.</param>
        [HttpPatch("{id}/description")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDescription(Guid id, UpdateProductDescriptionDto dto)
        {
            await _productService.UpdateDescriptionAsync(id, dto.Description);
            return NoContent();
        }

        private static ProductResponseDto ToResponseDto(Product product) => new()
        {
            Id = product.Id,
            Code = product.Code,
            Description = product.Description,
            Balance = product.Balance
        };
    }
}
