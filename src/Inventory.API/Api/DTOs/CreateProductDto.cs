using System.ComponentModel.DataAnnotations;

namespace Inventory.API.Api.DTOs
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(50)]
        public string? Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int Balance { get; set; }
    }
}
