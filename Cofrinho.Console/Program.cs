using Cofrinho.Console.Application.Interfaces;
using Cofrinho.Console.Application.Services;
using Cofrinho.Console.Domain.Entities;
using Cofrinho.Console.Domain.Interfaces;
using Cofrinho.Console.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Cofrinho.Console.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


var services = new ServiceCollection();

services.AddDbContext<CofrinhoDbContext>(opt =>
    opt.UseSqlite("Data Source=cofrinho.db"));

//  InMemory por EF:
services.AddScoped<IMetaRepository, EfMetaRepository>();

// Repositório (Infra)
// services.AddSingleton<IMetaRepository, InMemoryMetaRepository>();

// Application Service (Use Case)
services.AddSingleton<ICofrinhoAppService, CofrinhoAppService>();

var provider = services.BuildServiceProvider();

// Resolve o contrato, não a classe concreta
var service = provider.GetRequiredService<ICofrinhoAppService>();


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
                CriarMeta();
                break;

            case "2":
                ListarMetas();
                break;

            case "3":
                Depositar();
                break;

            case "4":
                Sacar();
                break;

            case "5":
                Extrato();
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
    var root = ex;

    while (root.InnerException is not null)
        root = root.InnerException;

    Pausar($"Erro: {root.GetType().Name} - {root.Message}");
}

}

void CriarMeta()
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

    service.CriarMeta(nome);
    Pausar("Meta criada com sucesso.");
}


void ListarMetas()
{
    Console.Clear();
    Console.WriteLine("=== Metas ===");

    var metas = service.ListarMetas();

    if (metas.Count == 0)
    {
        Pausar("Nenhuma meta cadastrada.");
        return;
    }

    foreach (var meta in metas)
    {
        Console.WriteLine($"- {meta.Nome} | Saldo: R$ {meta.Saldo:n2} | Transações: {meta.Transacoes.Count}");
    }

    Pausar("Fim da lista.");
}


void Depositar()
{
    Console.Clear();
    Console.WriteLine("=== Depositar ===");

    var nomeMeta = SelecionarMetaNome();
    if (nomeMeta is null) return;

    var valor = LerValor("Valor do depósito: ");
    if (valor is null) return;

    Console.Write("Descrição (opcional): ");
    var desc = Console.ReadLine();

    service.Depositar(nomeMeta, valor.Value, desc);
    Pausar("Depósito realizado com sucesso.");
}


void Sacar()
{
    Console.Clear();
    Console.WriteLine("=== Sacar ===");

    var nomeMeta = SelecionarMetaNome();
    if (nomeMeta is null) return;

    var valor = LerValor("Valor do saque: ");
    if (valor is null) return;

    Console.Write("Descrição (opcional): ");
    var desc = Console.ReadLine();

    service.Sacar(nomeMeta, valor.Value, desc);
    Pausar("Saque realizado com sucesso.");
}


void Extrato()
{
    Console.Clear();
    Console.WriteLine("=== Extrato ===");

    var nomeMeta = SelecionarMetaNome();
    if (nomeMeta is null) return;

    var extrato = service.GerarExtrato(nomeMeta);

    Console.WriteLine();
    Console.WriteLine(extrato);

    Pausar("Fim do extrato.");
}


string? SelecionarMetaNome()
{
    var metas = service.ListarMetas();

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

    // A validação final fica no Service quando tentar usar o nome.
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
    Console.WriteLine("Pressione ENTER para continuar...");
    Console.ReadLine();
}
