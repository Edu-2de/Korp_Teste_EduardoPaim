namespace Billing.API.Domain.Models
{
    public class Invoice
    {
        private readonly List<InvoiceItem> _items = [];

        public Guid Id { get; private set; }
        public int Number { get; private set; }
        public InvoiceStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

        public Invoice()
        {
            Id = Guid.NewGuid();
            Status = InvoiceStatus.Open;
            CreatedAt = DateTime.UtcNow;
        }

        public InvoiceItem? AddItem(Guid productId, int quantity)
        {
            if (Status != InvoiceStatus.Open)
            {
                throw new InvalidOperationException("Cannot add items to a closed invoice.");
            }

            var existingItem = _items.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.IncreaseQuantity(quantity);
                return null;
            }

            var item = new InvoiceItem(Id, productId, quantity);
            _items.Add(item);
            return item;
        }

        public void RemoveItem(Guid itemId)
        {
            if (Status != InvoiceStatus.Open)
            {
                throw new InvalidOperationException("Cannot remove items from a closed invoice.");
            }

            var item = _items.FirstOrDefault(i => i.Id == itemId)
                ?? throw new KeyNotFoundException($"Item {itemId} not found in this invoice.");

            _items.Remove(item);
        }

        public void Close()
        {
            if (Status != InvoiceStatus.Open)
            {
                throw new InvalidOperationException("Invoice is already closed.");
            }

            if (_items.Count == 0)
            {
                throw new InvalidOperationException("Cannot close an invoice without items.");
            }

            Status = InvoiceStatus.Closed;
        }
    }
}
