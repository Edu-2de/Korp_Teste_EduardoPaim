using System.Net;
using System.Net.Http.Json;
using Billing.API.Api.DTOs;
using Xunit;

namespace Billing.API.Tests.Controllers
{
    public class ControllerPrintInvoiceTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ControllerPrintInvoiceTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task POST_Print_ShouldReturn404_WhenInvoiceNotFound()
        {
            var response = await _client.PostAsync($"/api/invoices/{Guid.NewGuid()}/print", null);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task POST_Print_ShouldReturn409_WhenInvoiceIsEmpty()
        {
            var createResponse = await _client.PostAsync("/api/invoices", null);
            var created = await createResponse.Content.ReadFromJsonAsync<InvoiceResponseDto>();

            var response = await _client.PostAsync($"/api/invoices/{created!.Id}/print", null);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
    }
}
