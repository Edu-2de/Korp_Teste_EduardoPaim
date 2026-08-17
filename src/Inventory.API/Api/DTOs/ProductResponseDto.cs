namespace Inventory.API.Api.DTOs
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Balance { get; set; }
    }
}
