using System.Net;
using System.Net.Http.Json;
using Inventory.API.Api.DTOs;
using Xunit;

namespace Inventory.API.Tests.Controllers
{
    public class ControllerDecreaseBalanceTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ControllerDecreaseBalanceTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<HttpResponseMessage> PatchDecreaseBalance(Guid productId, DecreaseBalanceDto dto, string? idempotencyKey = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/products/{productId}/decrease-balance")
            {
                Content = JsonContent.Create(dto)
            };
            request.Headers.Add("X-Idempotency-Key", idempotencyKey ?? Guid.NewGuid().ToString());

            return await _client.SendAsync(request);
        }

        [Fact]
        public async Task PATCH_DecreaseBalance_ShouldReturn204_WhenValid()
        {
            var dto = new CreateProductDto { Code = $"P-{Guid.NewGuid()}", Description = "Test", Balance = 10 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            var response = await PatchDecreaseBalance(created!.Id, new DecreaseBalanceDto { Quantity = 3 });

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task PATCH_DecreaseBalance_ShouldReturn409_WhenInsufficientBalance()
        {
            var dto = new CreateProductDto { Code = $"P-{Guid.NewGuid()}", Description = "Test", Balance = 1 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            var response = await PatchDecreaseBalance(created!.Id, new DecreaseBalanceDto { Quantity = 999 });

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task PATCH_DecreaseBalance_ShouldReturn404_WhenProductNotFound()
        {
            var response = await PatchDecreaseBalance(Guid.NewGuid(), new DecreaseBalanceDto { Quantity = 1 });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PATCH_DecreaseBalance_ShouldReturn400_WhenQuantityIsZeroOrNegative()
        {
            var dto = new CreateProductDto { Code = $"P-{Guid.NewGuid()}", Description = "Test", Balance = 10 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            var response = await PatchDecreaseBalance(created!.Id, new DecreaseBalanceDto { Quantity = 0 });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
