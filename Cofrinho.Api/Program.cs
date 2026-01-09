using Cofrinho.Api.Logging;
using Cofrinho.Api.Middlewares;
using Cofrinho.Application;
using Cofrinho.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext() 
    .WriteTo.Console(new CofrinhoMinimalJsonFormatter())
    .CreateLogger();

builder.Host.UseSerilog();





// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Camadas
builder.Services.AddApplication();

// DB Path padronizado
var apiDir = builder.Environment.ContentRootPath;
var dbPath = Path.Combine(apiDir, "cofrinho.api.db");
builder.Services.AddInfrastructure($"Data Source={dbPath}");

builder.Services.AddHealthChecks()
    // Readiness: valida se consegue abrir o banco
    .AddDbContextCheck<Cofrinho.Infrastructure.Persistence.CofrinhoDbContext>(
        name: "db",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
        tags: new[] { "ready" })
    // Liveness: sempre OK se o processo está rodando
    .AddCheck("live", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(),
        tags: new[] { "live" });


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

// Migrate (dev)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Cofrinho.Infrastructure.Persistence.CofrinhoDbContext>();
    if (app.Environment.IsDevelopment())
        db.Database.Migrate();
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live"),
    ResponseWriter = WriteHealthResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready"),
    ResponseWriter = WriteHealthResponse
});


app.Run();


static Task WriteHealthResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";

    var payload = new
    {
        status = report.Status.ToString(),
        checks = report.Entries.Select(e => new
        {
            name = e.Key,
            status = e.Value.Status.ToString(),
            durationMs = e.Value.Duration.TotalMilliseconds
        })
    };

    return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
}
