using System.Net;
using System.Net.Http.Json;
using Billing.API.Api.DTOs;
using Xunit;

namespace Billing.API.Tests.Controllers
{
    public class ControllerDeleteInvoiceTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ControllerDeleteInvoiceTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task DELETE_Invoice_ShouldReturn204_WhenOpenAndValid()
        {
            var createResponse = await _client.PostAsync("/api/invoices", null);
            var created = await createResponse.Content.ReadFromJsonAsync<InvoiceResponseDto>();

            var response = await _client.DeleteAsync($"/api/invoices/{created!.Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var getResponse = await _client.GetAsync($"/api/invoices/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DELETE_Invoice_ShouldReturn404_WhenInvoiceNotFound()
        {
            var response = await _client.DeleteAsync($"/api/invoices/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
