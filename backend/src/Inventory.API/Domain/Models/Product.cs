using System.ComponentModel.DataAnnotations;

namespace Inventory.API.Domain.Models
{
    public class Product
    {
        public Guid Id { get; private set; }

        [MaxLength(50)]
        public string Code { get; private set; } = string.Empty;

        [MaxLength(200)]
        public string Description { get; private set; } = string.Empty;

        public int Balance { get; private set; }

        public bool IsActive { get; private set; } = true;

        private Product() { }

        public Product(string code, string description, int balance)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("Code is required.");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description is required.");
            }

            if (balance < 0)
            {
                throw new ArgumentException("Initial balance cannot be negative.");
            }

            Id = Guid.NewGuid();
            Code = code;
            Description = description;
            Balance = balance;
            IsActive = true;
        }

        public void UpdateDescription(string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
            {
                throw new ArgumentException("Description is required.");
            }

            Description = newDescription;
        }

        public void UpdateBalance(int newBalance)
        {
            if (newBalance < 0)
            {
                throw new ArgumentException("Balance cannot be negative.");
            }

            Balance = newBalance;
        }

        public void DecreaseBalance(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.");
            }

            if (Balance < quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient balance. Available: {Balance}, requested: {quantity}");
            }

            Balance -= quantity;
        }

        public void Deactivate()
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("Product is already inactive.");
            }

            IsActive = false;
        }
    }
}
