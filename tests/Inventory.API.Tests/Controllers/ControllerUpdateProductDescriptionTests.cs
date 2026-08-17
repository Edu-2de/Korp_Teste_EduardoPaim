using System.Net;
using System.Net.Http.Json;
using Inventory.API.Api.DTOs;
using Xunit;

namespace Inventory.API.Tests.Controllers
{
    public class ControllerUpdateProductDescriptionTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ControllerUpdateProductDescriptionTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PATCH_UpdateDescription_ShouldReturn204_WhenValid()
        {
            var dto = new CreateProductDto { Code = $"P-{Guid.NewGuid()}", Description = "Old", Balance = 10 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            var response = await _client.PatchAsJsonAsync(
                $"/api/products/{created!.Id}/description",
                new UpdateProductDescriptionDto { Description = "New Description" });

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var getResponse = await _client.GetAsync($"/api/products/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<ProductResponseDto>();
            Assert.Equal("New Description", updated!.Description);
        }

        [Fact]
        public async Task PATCH_UpdateDescription_ShouldReturn404_WhenProductNotFound()
        {
            var response = await _client.PatchAsJsonAsync(
                $"/api/products/{Guid.NewGuid()}/description",
                new UpdateProductDescriptionDto { Description = "Any" });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PATCH_UpdateDescription_ShouldReturn400_WhenDescriptionIsEmpty()
        {
            var dto = new CreateProductDto { Code = $"P-{Guid.NewGuid()}", Description = "Old", Balance = 10 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            var response = await _client.PatchAsJsonAsync(
                $"/api/products/{created!.Id}/description",
                new UpdateProductDescriptionDto { Description = "" });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
