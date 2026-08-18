namespace Billing.API.Api.DTOs
{
    public class InvoiceItemResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
