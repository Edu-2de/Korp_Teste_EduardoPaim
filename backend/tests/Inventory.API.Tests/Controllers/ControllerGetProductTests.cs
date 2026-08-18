using System.Net;
using System.Net.Http.Json;
using Inventory.API.Api.DTOs;
using Xunit;

namespace Inventory.API.Tests.Controllers
{
    public class ControllerGetProductTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ControllerGetProductTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GET_ProductById_ShouldReturn404_WhenNotFound()
        {
            var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GET_ProductById_ShouldReturn200_WhenFound()
        {
            var dto = new CreateProductDto { Code = $"P-{Guid.NewGuid()}", Description = "Test", Balance = 10 };
            var createResponse = await _client.PostAsJsonAsync("/api/products", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            var response = await _client.GetAsync($"/api/products/{created!.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GET_AllProducts_ShouldReturn200()
        {
            var response = await _client.GetAsync("/api/products");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
