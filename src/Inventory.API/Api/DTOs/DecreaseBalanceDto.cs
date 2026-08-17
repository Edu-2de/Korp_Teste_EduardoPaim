using System.ComponentModel.DataAnnotations;

namespace Inventory.API.Api.DTOs
{
    public class DecreaseBalanceDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }
    }
}
