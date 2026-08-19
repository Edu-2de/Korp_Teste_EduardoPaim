namespace Billing.API.Domain.Models
{
    public class InvoiceItem
    {
        public Guid Id { get; private set; }
        public Guid InvoiceId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }

        private InvoiceItem() { }

        public InvoiceItem(Guid invoiceId, Guid productId, int quantity)
        {
            if (productId == Guid.Empty)
            {
                throw new ArgumentException("ProductId is required.");
            }

            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.");
            }

            Id = Guid.NewGuid();
            InvoiceId = invoiceId;
            ProductId = productId;
            Quantity = quantity;
        }

        public void IncreaseQuantity(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.");
            }

            Quantity += amount;
        }
    }
}
