using Cofrinho.Api.Middlewares;
using Cofrinho.Application;
using Cofrinho.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Cofrinho.Api.Logging;

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

app.Run();
