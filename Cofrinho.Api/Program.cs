using Cofrinho.Application;
using Cofrinho.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Camadas
builder.Services.AddApplication();

var conn = builder.Configuration.GetConnectionString("CofrinhoDb")!;
builder.Services.AddInfrastructure(conn);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<Cofrinho.Api.Middlewares.GlobalExceptionMiddleware>();
app.MapControllers();

// (opcional) garantir DB criado, se você ainda estiver com EnsureCreated
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Cofrinho.Infrastructure.Persistence.CofrinhoDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
