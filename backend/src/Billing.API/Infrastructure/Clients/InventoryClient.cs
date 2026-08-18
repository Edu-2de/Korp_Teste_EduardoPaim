using System.Net.Http.Json;

namespace Billing.API.Infrastructure.Clients
{
    public class InventoryClient(HttpClient httpClient) : IInventoryClient
    {
        public async Task DecreaseBalanceAsync(Guid productId, int quantity, string idempotencyKey)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/products/{productId}/decrease-balance")
            {
                Content = JsonContent.Create(new { quantity })
            };
            request.Headers.Add("X-Idempotency-Key", idempotencyKey);

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return;

            var errorBody = await response.Content.ReadAsStringAsync();

            throw response.StatusCode switch
            {
                System.Net.HttpStatusCode.NotFound =>
                    new KeyNotFoundException($"Product {productId} not found in Inventory service."),
                System.Net.HttpStatusCode.Conflict =>
                    new InvalidOperationException($"Insufficient balance for product {productId}."),
                _ =>
                    new HttpRequestException($"Inventory service returned {response.StatusCode}: {errorBody}")
            };
        }
    }
}
