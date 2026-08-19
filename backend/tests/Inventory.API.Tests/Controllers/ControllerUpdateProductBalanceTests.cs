using System.Net;
using System.Net.Http.Json;
using Inventory.API.Api.DTOs;
using Xunit;

namespace Inventory.API.Tests.Controllers
{
    public class ControllerUpdateProductBalanceTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ControllerUpdateProductBalanceTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PATCH_UpdateBalance_ShouldReturn204_WhenValid()
        {
            var dto = new CreateProductDto { Code = $"P-{Guid.NewGuid()}", Description = "Old", Balance = 10 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            var response = await _client.PatchAsJsonAsync(
                $"/api/products/{created!.Id}/balance",
                new UpdateProductBalanceDto { Balance = 42 });

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var getResponse = await _client.GetAsync($"/api/products/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<ProductResponseDto>();
            Assert.Equal(42, updated!.Balance);
        }

        [Fact]
        public async Task PATCH_UpdateBalance_ShouldReturn404_WhenProductNotFound()
        {
            var response = await _client.PatchAsJsonAsync(
                $"/api/products/{Guid.NewGuid()}/balance",
                new UpdateProductBalanceDto { Balance = 5 });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PATCH_UpdateBalance_ShouldReturn400_WhenBalanceIsNegative()
        {
            var dto = new CreateProductDto { Code = $"P-{Guid.NewGuid()}", Description = "Old", Balance = 10 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            var response = await _client.PatchAsJsonAsync(
                $"/api/products/{created!.Id}/balance",
                new UpdateProductBalanceDto { Balance = -1 });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
