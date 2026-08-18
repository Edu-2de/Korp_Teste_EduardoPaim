namespace Inventory.API.Domain.Models
{
    public class IdempotencyRecord
    {
        public string Key { get; private set; } = string.Empty;
        public DateTime ProcessedAt { get; private set; }

        private IdempotencyRecord() { }

        public IdempotencyRecord(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Idempotency key is required.");
            }

            Key = key;
            ProcessedAt = DateTime.UtcNow;
        }
    }
}
