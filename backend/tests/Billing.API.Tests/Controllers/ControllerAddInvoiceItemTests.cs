using System.Net;
using System.Net.Http.Json;
using Billing.API.Api.DTOs;
using Xunit;

namespace Billing.API.Tests.Controllers
{
    public class ControllerAddInvoiceItemTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ControllerAddInvoiceItemTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task POST_AddItem_ShouldReturn204_WhenValid()
        {
            var createResponse = await _client.PostAsync("/api/invoices", null);
            var created = await createResponse.Content.ReadFromJsonAsync<InvoiceResponseDto>();

            var response = await _client.PostAsJsonAsync(
                $"/api/invoices/{created!.Id}/items",
                new AddInvoiceItemDto { ProductId = Guid.NewGuid(), Quantity = 2 });

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task POST_AddItem_ShouldReturn404_WhenInvoiceNotFound()
        {
            var response = await _client.PostAsJsonAsync(
                $"/api/invoices/{Guid.NewGuid()}/items",
                new AddInvoiceItemDto { ProductId = Guid.NewGuid(), Quantity = 1 });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task POST_AddItem_ShouldReturn400_WhenQuantityIsZeroOrNegative()
        {
            var createResponse = await _client.PostAsync("/api/invoices", null);
            var created = await createResponse.Content.ReadFromJsonAsync<InvoiceResponseDto>();

            var response = await _client.PostAsJsonAsync(
                $"/api/invoices/{created!.Id}/items",
                new AddInvoiceItemDto { ProductId = Guid.NewGuid(), Quantity = 0 });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_AddItem_ShouldReturn400_WhenProductIdIsEmpty()
        {
            var createResponse = await _client.PostAsync("/api/invoices", null);
            var created = await createResponse.Content.ReadFromJsonAsync<InvoiceResponseDto>();

            var response = await _client.PostAsJsonAsync(
                $"/api/invoices/{created!.Id}/items",
                new AddInvoiceItemDto { ProductId = Guid.Empty, Quantity = 1 });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task POST_AddItem_ShouldConsolidateQuantity_WhenSameProductAddedTwice()
        {
            var createResponse = await _client.PostAsync("/api/invoices", null);
            var created = await createResponse.Content.ReadFromJsonAsync<InvoiceResponseDto>();
            var productId = Guid.NewGuid();

            await _client.PostAsJsonAsync(
                $"/api/invoices/{created!.Id}/items",
                new AddInvoiceItemDto { ProductId = productId, Quantity = 2 });
            await _client.PostAsJsonAsync(
                $"/api/invoices/{created.Id}/items",
                new AddInvoiceItemDto { ProductId = productId, Quantity = 3 });

            var getResponse = await _client.GetAsync($"/api/invoices/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<InvoiceResponseDto>();

            Assert.Single(updated!.Items);
            Assert.Equal(5, updated.Items.First().Quantity);
        }
    }
}
