using Microsoft.AspNetCore.Mvc;
using Billing.API.Api.DTOs;
using Billing.API.Application.Services;
using Billing.API.Domain.Models;
using Shared.Kernel;

namespace Billing.API.Api.Controllers
{
    /// <summary>
    /// Gerencia a criação, consulta e composição de notas fiscais.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController(IInvoiceService invoiceService) : ControllerBase
    {
        private readonly IInvoiceService _invoiceService = invoiceService;

        /// <summary>
        /// Lista todas as notas fiscais cadastradas, com seus respectivos itens.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InvoiceResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetAll()
        {
            var invoices = await _invoiceService.GetAllAsync();
            return Ok(invoices.Select(ToResponseDto));
        }

        /// <summary>
        /// Busca uma nota fiscal pelo seu identificador, incluindo seus itens.
        /// </summary>
        /// <param name="id">Identificador (GUID) da nota fiscal.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InvoiceResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoiceResponseDto>> GetById(Guid id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Invoice {id} not found.");

            return Ok(ToResponseDto(invoice));
        }

        /// <summary>
        /// Cria uma nova nota fiscal vazia, com numeração sequencial automática
        /// e status inicial "Open" (Aberta).
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(InvoiceResponseDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<InvoiceResponseDto>> Create()
        {
            var invoice = await _invoiceService.CreateAsync();
            return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, ToResponseDto(invoice));
        }

        /// <summary>
        /// Adiciona um item (produto + quantidade) a uma nota fiscal existente.
        /// Só é permitido enquanto a nota estiver com status "Open" (Aberta).
        /// </summary>
        /// <param name="id">Identificador (GUID) da nota fiscal.</param>
        /// <param name="dto">Produto e quantidade a serem adicionados.</param>
        [HttpPost("{id}/items")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddItem(Guid id, AddInvoiceItemDto dto)
        {
            await _invoiceService.AddItemAsync(id, dto.ProductId, dto.Quantity);
            return NoContent();
        }

        private static InvoiceResponseDto ToResponseDto(Invoice invoice) => new()
        {
            Id = invoice.Id,
            Number = invoice.Number,
            Status = invoice.Status.ToString(),
            CreatedAt = invoice.CreatedAt,
            Items = invoice.Items.Select(item => new InvoiceItemResponseDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };

        /// <summary>
        /// Imprime uma nota fiscal: debita o saldo de cada produto no Inventory.API
        /// e atualiza o status da nota para "Closed" (Fechada). Só é permitido para
        /// notas com status "Open" (Aberta).
        /// </summary>
        /// <param name="id">Identificador (GUID) da nota fiscal.</param>
        [HttpPost("{id}/print")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Print(Guid id)
        {
            await _invoiceService.PrintAsync(id);
            return NoContent();
        }
    }
}
