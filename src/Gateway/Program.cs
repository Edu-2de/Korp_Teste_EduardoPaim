using Shared.Kernel.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.UseSharedSerilog("Gateway");
builder.Services.AddSharedCorrelationId();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSharedCorrelationId();
app.UseCors("AllowAngular");

app.MapReverseProxy();

app.Run();
