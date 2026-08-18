using System.ComponentModel.DataAnnotations;

namespace Billing.API.Api.DTOs
{
    public class AddInvoiceItemDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }
    }
}
