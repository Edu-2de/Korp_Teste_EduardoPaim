using System.Net;
using System.Net.Http.Json;
using Billing.API.Api.DTOs;
using Xunit;

namespace Billing.API.Tests.Controllers
{
    public class ControllerGetInvoiceTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ControllerGetInvoiceTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GET_InvoiceById_ShouldReturn404_WhenNotFound()
        {
            var response = await _client.GetAsync($"/api/invoices/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GET_InvoiceById_ShouldReturn200_WhenFound()
        {
            var createResponse = await _client.PostAsync("/api/invoices", null);
            var created = await createResponse.Content.ReadFromJsonAsync<InvoiceResponseDto>();

            var response = await _client.GetAsync($"/api/invoices/{created!.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GET_AllInvoices_ShouldReturn200()
        {
            var response = await _client.GetAsync("/api/invoices");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
