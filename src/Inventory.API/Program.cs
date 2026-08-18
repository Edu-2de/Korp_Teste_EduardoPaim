using Inventory.API.Infrastructure.Data;
using Inventory.API.Application.Services;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.UseSharedSerilog("Inventory.API");
builder.Services.AddSharedCorrelationId();

builder.Services.AddControllers();
builder.Services.AddStandardizedValidationErrors();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Inventory API - Korp Teste",
        Version = "v1",
        Description = "Microsserviço responsável pelo controle de produtos e saldo de estoque."
    });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseSharedCorrelationId();
app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program { }
