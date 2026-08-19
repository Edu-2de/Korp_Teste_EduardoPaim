using System.ComponentModel.DataAnnotations;

namespace Inventory.API.Api.DTOs
{
    public class UpdateProductBalanceDto
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Balance cannot be negative.")]
        public int Balance { get; set; }
    }
}
