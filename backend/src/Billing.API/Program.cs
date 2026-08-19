using Billing.API.Infrastructure.Data;
using Billing.API.Application.Services;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Extensions;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Billing.API.Infrastructure.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.UseSharedSerilog("Billing.API");
builder.Services.AddSharedCorrelationId();

builder.Services.AddControllers();
builder.Services.AddStandardizedValidationErrors();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Billing API - Korp Teste",
        Version = "v1",
        Description = "Microsserviço responsável pela gestão de notas fiscais."
    });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<IInvoiceService, InvoiceService>();

builder.Services.AddDbContext<BillingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var inventoryApiBaseUrl = builder.Configuration["InventoryApi:BaseUrl"]
    ?? throw new InvalidOperationException("Configuration 'InventoryApi:BaseUrl' is missing.");

builder.Services.AddHttpClient<IInventoryClient, InventoryClient>(client =>
{
    client.BaseAddress = new Uri(inventoryApiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());

var app = builder.Build();

app.UseSharedCorrelationId();
app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
    await db.Database.MigrateAsync();
}

app.MapControllers();

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(30));
}

public partial class Program { }
