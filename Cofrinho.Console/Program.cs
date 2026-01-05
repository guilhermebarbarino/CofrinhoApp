using Cofrinho.Console.Application.Interfaces;
using Cofrinho.Console.Application.Services.UseCases;
using Cofrinho.Console.Domain.Interfaces;
using Cofrinho.Console.Infrastructure.Persistence;
using Cofrinho.Console.Infrastructure.Repositories;
using Cofrinho.Console.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var services = new ServiceCollection();

services.AddLogging(cfg =>
{
    cfg.ClearProviders();
    cfg.AddConsole();
    cfg.SetMinimumLevel(LogLevel.Information);
});

var correlationId = Guid.NewGuid().ToString("N");


var dbPath = Path.Combine(AppContext.BaseDirectory, "cofrinho.db");
services.AddDbContext<CofrinhoDbContext>(opt =>
    opt.UseSqlite($"Data Source={dbPath}"));

services.AddScoped<IMetaRepository, EfMetaRepository>();
services.AddScoped<IUnitOfWork, EfUnitOfWork>();

services.AddScoped<ICriarMetaUseCase, CriarMetaUseCase>();
services.AddScoped<IDepositarUseCase, DepositarUseCase>();
services.AddScoped<ISacarUseCase, SacarUseCase>();
services.AddScoped<IListarMetasUseCase, ListarMetasUseCase>();
services.AddScoped<IGerarExtratoUseCase, GerarExtratoUseCase>();

var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

var criarMetaUseCase = scope.ServiceProvider.GetRequiredService<ICriarMetaUseCase>();
var listarMetasUseCase = scope.ServiceProvider.GetRequiredService<IListarMetasUseCase>();
var depositarUseCase = scope.ServiceProvider.GetRequiredService<IDepositarUseCase>();
var sacarUseCase = scope.ServiceProvider.GetRequiredService<ISacarUseCase>();
var gerarExtratoUseCase = scope.ServiceProvider.GetRequiredService<IGerarExtratoUseCase>();
var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
    .CreateLogger("Cofrinho.Console");




while (true)
{
    Console.Clear();
    Console.WriteLine("=== COFRINHO (Console) ===");
    Console.WriteLine("1) Criar meta");
    Console.WriteLine("2) Listar metas");
    Console.WriteLine("3) Depositar");
    Console.WriteLine("4) Sacar");
    Console.WriteLine("5) Extrato");
    Console.WriteLine("6) Sair");
    Console.WriteLine();
    Console.Write("Escolha uma opção: ");

    var opcao = Console.ReadLine()?.Trim();

    try
    {
        switch (opcao)
        {
            case "1":
                await CriarMetaAsync();
                break;

            case "2":
                await ListarMetasAsync();
                break;

            case "3":
                await DepositarAsync();
                break;

            case "4":
                await SacarAsync();
                break;

            case "5":
                await ExtratoAsync();
                break;

            case "6":
                return;

            default:
                Pausar("Opção inválida.");
                break;
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro no fluxo. CorrelationId={CorrelationId}", correlationId);

        var msg = ex switch
        {
            ArgumentException => ex.Message,
            InvalidOperationException => ex.Message,
            KeyNotFoundException => ex.Message,
            _ => "Ocorreu um erro inesperado."
        };

        Pausar($"Erro: {msg}");
    }
}


async Task CriarMetaAsync()
{
    Console.Clear();
    Console.WriteLine("=== Criar Meta ===");
    Console.Write("Nome da meta: ");
    var nome = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(nome))
    {
        Pausar("Nome da meta é obrigatório.");
        return;
    }

    await criarMetaUseCase.ExecuteAsync(nome);
    Pausar("Meta criada com sucesso.");
}



async Task ListarMetasAsync()
{
    Console.Clear();
    Console.WriteLine("=== Metas ===");

    var metas = await listarMetasUseCase.ExecuteAsync();

    if (metas.Count == 0)
    {
        Pausar("Nenhuma meta cadastrada.");
        return;
    }

    foreach (var meta in metas)
        Console.WriteLine($"- {meta.Nome} | Saldo: R$ {meta.Saldo:n2} | Transações: {meta.Transacoes.Count}");

    Pausar("Fim da lista.");
}



async Task DepositarAsync()
{
    Console.Clear();
    Console.WriteLine("=== Depositar ===");

    var nomeMeta = await SelecionarMetaNomeAsync();
    if (nomeMeta is null) return;

    var valor = LerValor("Valor do depósito: ");
    if (valor is null) return;

    Console.Write("Descrição (opcional): ");
    var desc = Console.ReadLine();

    await depositarUseCase.ExecuteAsync(nomeMeta, valor.Value, desc);
    Pausar("Depósito realizado com sucesso.");
}



async Task SacarAsync()
{
    Console.Clear();
    Console.WriteLine("=== Sacar ===");

    var nomeMeta = await SelecionarMetaNomeAsync();
    if (nomeMeta is null) return;

    var valor = LerValor("Valor do saque: ");
    if (valor is null) return;

    Console.Write("Descrição (opcional): ");
    var desc = Console.ReadLine();

    await sacarUseCase.ExecuteAsync(nomeMeta, valor.Value, desc);
    Pausar("Saque realizado com sucesso.");
}



async Task ExtratoAsync()
{
    Console.Clear();
    Console.WriteLine("=== Extrato ===");

    var nomeMeta = await SelecionarMetaNomeAsync();
    if (nomeMeta is null) return;

    var texto = await gerarExtratoUseCase.ExecuteAsync(nomeMeta);

    Console.WriteLine();
    Console.WriteLine(texto);

    Pausar("Fim do extrato.");
}



async Task<string?> SelecionarMetaNomeAsync()
{
    var metas = await listarMetasUseCase.ExecuteAsync();

    if (metas.Count == 0)
    {
        Pausar("Nenhuma meta cadastrada. Crie uma meta primeiro.");
        return null;
    }

    Console.WriteLine("Metas disponíveis:");
    foreach (var m in metas)
        Console.WriteLine($"- {m.Nome}");

    Console.WriteLine();
    Console.Write("Digite o nome da meta: ");
    var nomeMeta = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(nomeMeta))
    {
        Pausar("Nome da meta é obrigatório.");
        return null;
    }

    return nomeMeta;
}



decimal? LerValor(string label)
{
    Console.Write(label);
    var input = Console.ReadLine()?.Trim();

    if (!decimal.TryParse(input, out var valor))
    {
        Pausar("Valor inválido. Exemplo: 10,50");
        return null;
    }

    if (valor <= 0)
    {
        Pausar("O valor deve ser maior que zero.");
        return null;
    }

    return valor;
}

void Pausar(string msg)
{
    Console.WriteLine();
    Console.WriteLine(msg);
    Console.WriteLine("DB Path: " + Path.GetFullPath("cofrinho.db"));
    Console.WriteLine("Pressione ENTER para continuar...");
    Console.ReadLine();
}
