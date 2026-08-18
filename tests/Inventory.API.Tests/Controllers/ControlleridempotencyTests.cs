using System.Net;
using System.Net.Http.Json;
using Inventory.API.Api.DTOs;
using Xunit;

namespace Inventory.API.Tests.Controllers
{
    public class ControllerIdempotencyTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ControllerIdempotencyTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<HttpResponseMessage> PatchDecreaseBalance(Guid productId, DecreaseBalanceDto dto, string idempotencyKey)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/products/{productId}/decrease-balance")
            {
                Content = JsonContent.Create(dto)
            };
            request.Headers.Add("X-Idempotency-Key", idempotencyKey);

            return await _client.SendAsync(request);
        }

        [Fact]
        public async Task PATCH_DecreaseBalance_ShouldReturn400_WhenIdempotencyKeyHeaderIsMissing()
        {
            var dto = new CreateProductDto { Code = $"P-{Guid.NewGuid()}", Description = "Test", Balance = 10 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            var response = await _client.PatchAsJsonAsync(
                $"/api/products/{created!.Id}/decrease-balance",
                new DecreaseBalanceDto { Quantity = 1 });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PATCH_DecreaseBalance_ShouldNotDecreaseTwice_WhenSameIdempotencyKeyIsReused()
        {
            var dto = new CreateProductDto { Code = $"P-{Guid.NewGuid()}", Description = "Test", Balance = 10 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<ProductResponseDto>();
            var key = Guid.NewGuid().ToString();

            var first = await PatchDecreaseBalance(created!.Id, new DecreaseBalanceDto { Quantity = 3 }, key);
            var second = await PatchDecreaseBalance(created.Id, new DecreaseBalanceDto { Quantity = 3 }, key);

            Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, second.StatusCode);

            var getResponse = await _client.GetAsync($"/api/products/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<ProductResponseDto>();
            Assert.Equal(7, updated!.Balance);
        }

        [Fact]
        public async Task PATCH_DecreaseBalance_ShouldDecreaseIndependently_WhenDifferentIdempotencyKeysAreUsed()
        {
            var dto = new CreateProductDto { Code = $"P-{Guid.NewGuid()}", Description = "Test", Balance = 10 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            await PatchDecreaseBalance(created!.Id, new DecreaseBalanceDto { Quantity = 3 }, Guid.NewGuid().ToString());
            await PatchDecreaseBalance(created.Id, new DecreaseBalanceDto { Quantity = 2 }, Guid.NewGuid().ToString());

            var getResponse = await _client.GetAsync($"/api/products/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<ProductResponseDto>();
            Assert.Equal(5, updated!.Balance);
        }
    }
}
