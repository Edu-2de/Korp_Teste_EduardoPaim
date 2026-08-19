using System.Net;
using System.Net.Http.Json;
using Billing.API.Api.DTOs;
using Xunit;

namespace Billing.API.Tests.Controllers
{
    public class ControllerRemoveInvoiceItemTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ControllerRemoveInvoiceItemTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task DELETE_RemoveItem_ShouldReturn204_WhenValid()
        {
            var createResponse = await _client.PostAsync("/api/invoices", null);
            var created = await createResponse.Content.ReadFromJsonAsync<InvoiceResponseDto>();

            await _client.PostAsJsonAsync(
                $"/api/invoices/{created!.Id}/items",
                new AddInvoiceItemDto { ProductId = Guid.NewGuid(), Quantity = 2 });

            var invoice = await (await _client.GetAsync($"/api/invoices/{created.Id}"))
                .Content.ReadFromJsonAsync<InvoiceResponseDto>();
            var itemId = invoice!.Items.First().Id;

            var response = await _client.DeleteAsync($"/api/invoices/{created.Id}/items/{itemId}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var afterDelete = await (await _client.GetAsync($"/api/invoices/{created.Id}"))
                .Content.ReadFromJsonAsync<InvoiceResponseDto>();
            Assert.Empty(afterDelete!.Items);
        }

        [Fact]
        public async Task DELETE_RemoveItem_ShouldReturn404_WhenInvoiceNotFound()
        {
            var response = await _client.DeleteAsync($"/api/invoices/{Guid.NewGuid()}/items/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DELETE_RemoveItem_ShouldReturn404_WhenItemNotFound()
        {
            var createResponse = await _client.PostAsync("/api/invoices", null);
            var created = await createResponse.Content.ReadFromJsonAsync<InvoiceResponseDto>();

            var response = await _client.DeleteAsync($"/api/invoices/{created!.Id}/items/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
