
using Cofrinho.Application.Interfaces;
using Cofrinho.Application.Services.UseCases;
using Cofrinho.Application.UseCases;
using Cofrinho.Console.Application.Services.UseCases;
using Cofrinho.Domain.Interfaces;
using Cofrinho.Infrastructure.Persistence;
using Cofrinho.Infrastructure.Repositories;
using Cofrinho.Infrastructure.UnitOfWork;
using FluentValidation;
using FluentValidation.AspNetCore;
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
builder.Services.AddScoped<IGerarExtratoGeralUseCase, GerarExtratoGeralUseCase>();
builder.Services.AddScoped<IObterMetaPorNomeUseCase, ObterMetaPorNomeUseCase>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.UseMiddleware<Cofrinho.Api.Middlewares.GlobalExceptionMiddleware>();


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
