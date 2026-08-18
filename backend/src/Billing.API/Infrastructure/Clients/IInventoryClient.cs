namespace Billing.API.Infrastructure.Clients
{
    public interface IInventoryClient
    {
        Task DecreaseBalanceAsync(Guid productId, int quantity, string idempotencyKey);
    }
}
