namespace Billing.API.Domain.Models
{
    public class Invoice
    {
        private readonly List<InvoiceItem> _items = new();

        public Guid Id { get; private set; }
        public int Number { get; private set; }
        public InvoiceStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

        private Invoice() { }

        public Invoice(int number)
        {
            if (number <= 0) { throw new ArgumentException("Number must be greater than zero."); }

            Id = Guid.NewGuid();
            Number = number;
            Status = InvoiceStatus.Open;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddItem(Guid productId, int quantity)
        {
            if (Status != InvoiceStatus.Open)
            {
                throw new InvalidOperationException("Cannot add items to a closed invoice.");
            }

            var item = new InvoiceItem(Id, productId, quantity);
            _items.Add(item);
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
