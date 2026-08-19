using System.Net;
using System.Net.Http.Json;
using Inventory.API.Api.DTOs;
using Xunit;

namespace Inventory.API.Tests.Controllers
{
    public class ControllerDeactivateProductTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ControllerDeactivateProductTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task DELETE_Deactivate_ShouldReturn204_AndMarkProductInactive_WhenValid()
        {
            var dto = new CreateProductDto { Code = $"P-{Guid.NewGuid()}", Description = "Old", Balance = 10 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            var response = await _client.DeleteAsync($"/api/products/{created!.Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var getResponse = await _client.GetAsync($"/api/products/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<ProductResponseDto>();
            Assert.False(updated!.IsActive);
            Assert.Equal(10, updated.Balance);
        }

        [Fact]
        public async Task DELETE_Deactivate_ShouldReturn404_WhenProductNotFound()
        {
            var response = await _client.DeleteAsync($"/api/products/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DELETE_Deactivate_ShouldReturn409_WhenAlreadyInactive()
        {
            var dto = new CreateProductDto { Code = $"P-{Guid.NewGuid()}", Description = "Old", Balance = 10 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            await _client.DeleteAsync($"/api/products/{created!.Id}");
            var response = await _client.DeleteAsync($"/api/products/{created.Id}");

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task POST_Create_ShouldAllowReusingCode_FromDeactivatedProduct()
        {
            var code = $"P-{Guid.NewGuid()}";
            var firstDto = new CreateProductDto { Code = code, Description = "First", Balance = 5 };
            var firstResponse = await _client.PostAsJsonAsync("/api/products", firstDto);
            var first = await firstResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            await _client.DeleteAsync($"/api/products/{first!.Id}");

            var secondDto = new CreateProductDto { Code = code, Description = "Second", Balance = 7 };
            var secondResponse = await _client.PostAsJsonAsync("/api/products", secondDto);

            Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);
        }
    }
}
