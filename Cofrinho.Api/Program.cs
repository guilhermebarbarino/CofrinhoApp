using Cofrinho.Console.Application.Interfaces;
using Cofrinho.Console.Application.Services.UseCases;
using Cofrinho.Console.Domain.Interfaces;
using Cofrinho.Console.Infrastructure.Persistence;
using Cofrinho.Console.Infrastructure.Repositories;
using Cofrinho.Console.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetConnectionString("CofrinhoDb");

// DB
builder.Services.AddDbContext<CofrinhoDbContext>(opt => opt.UseSqlite(conn));

// Infra
builder.Services.AddScoped<IMetaRepository, EfMetaRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

// UseCases
builder.Services.AddScoped<ICriarMetaUseCase, CriarMetaUseCase>();
builder.Services.AddScoped<IDepositarUseCase, DepositarUseCase>();
builder.Services.AddScoped<ISacarUseCase, SacarUseCase>();
builder.Services.AddScoped<IListarMetasUseCase, ListarMetasUseCase>();
builder.Services.AddScoped<IGerarExtratoUseCase, GerarExtratoUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CofrinhoDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
