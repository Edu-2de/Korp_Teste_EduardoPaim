using System.Net;
using System.Net.Http.Json;
using Inventory.API.Api.DTOs;
using Xunit;

namespace Inventory.API.Tests.Controllers
{
    public class ControllerCreateProductTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ControllerCreateProductTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task POST_Products_ShouldReturn201_WhenValid()
        {
            var dto = new CreateProductDto { Code = $"P-{Guid.NewGuid()}", Description = "Test", Balance = 10 };

            var response = await _client.PostAsJsonAsync("/api/products", dto);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<ProductResponseDto>();
            Assert.NotNull(created);
            Assert.Equal(dto.Code, created!.Code);
        }

        [Fact]
        public async Task POST_Products_ShouldReturn400_WhenCodeIsNotProvided()
        {
            var dto = new CreateProductDto { Description = "Sem código", Balance = 5 };

            var response = await _client.PostAsJsonAsync("/api/products", dto);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_Products_ShouldReturn409_WhenCodeAlreadyExists()
        {
            var code = $"P-{Guid.NewGuid()}";
            var dto = new CreateProductDto { Code = code, Description = "Test", Balance = 10 };
            await _client.PostAsJsonAsync("/api/products", dto);

            var response = await _client.PostAsJsonAsync("/api/products", dto);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task POST_Products_ShouldReturn400_WhenBalanceIsNegative()
        {
            var dto = new CreateProductDto { Code = $"P-{Guid.NewGuid()}", Description = "Test", Balance = -5 };

            var response = await _client.PostAsJsonAsync("/api/products", dto);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
