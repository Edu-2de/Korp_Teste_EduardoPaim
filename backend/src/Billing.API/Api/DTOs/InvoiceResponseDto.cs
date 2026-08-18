namespace Billing.API.Api.DTOs
{
    public class InvoiceResponseDto
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<InvoiceItemResponseDto> Items { get; set; } = new();
    }
}
