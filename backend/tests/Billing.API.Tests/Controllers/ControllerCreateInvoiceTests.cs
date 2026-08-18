using System.Net;
using System.Net.Http.Json;
using Billing.API.Api.DTOs;
using Xunit;

namespace Billing.API.Tests.Controllers
{
    public class ControllerCreateInvoiceTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ControllerCreateInvoiceTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task POST_Invoices_ShouldReturn201_WithOpenStatusAndNoItems()
        {
            var response = await _client.PostAsync("/api/invoices", null);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<InvoiceResponseDto>();
            Assert.NotNull(created);
            Assert.Equal("Open", created!.Status);
            Assert.Empty(created.Items);
        }

        [Fact]
        public async Task POST_Invoices_ShouldGenerateSequentialNumber()
        {
            var first = await _client.PostAsync("/api/invoices", null);
            var second = await _client.PostAsync("/api/invoices", null);

            var firstInvoice = await first.Content.ReadFromJsonAsync<InvoiceResponseDto>();
            var secondInvoice = await second.Content.ReadFromJsonAsync<InvoiceResponseDto>();

            Assert.True(secondInvoice!.Number > firstInvoice!.Number);
        }
    }
}
