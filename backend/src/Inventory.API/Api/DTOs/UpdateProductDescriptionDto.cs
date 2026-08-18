using System.ComponentModel.DataAnnotations;

namespace Inventory.API.Api.DTOs
{
    public class UpdateProductDescriptionDto
    {
        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
    }
}
