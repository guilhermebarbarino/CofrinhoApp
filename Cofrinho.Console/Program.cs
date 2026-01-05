using Cofrinho.Console.Domain.Entities;
using Cofrinho.Console.Domain.Services;

var service = new CofrinhoService();
var metas = new Dictionary<string, Meta>(StringComparer.OrdinalIgnoreCase);

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
        // Console app: feedback direto. Depois evoluímos para middleware de erro na API.
        Pausar($"Erro: {ex.Message}");
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

    if (metas.ContainsKey(nome))
    {
        Pausar("Já existe uma meta com esse nome.");
        return;
    }

    var meta = service.CriarMeta(nome);
    metas[nome] = meta;

    Pausar("Meta criada com sucesso.");
}

void ListarMetas()
{
    Console.Clear();
    Console.WriteLine("=== Metas ===");

    if (metas.Count == 0)
    {
        Pausar("Nenhuma meta cadastrada.");
        return;
    }

    foreach (var meta in metas.Values.OrderBy(m => m.Nome))
    {
        Console.WriteLine($"- {meta.Nome} | Saldo: R$ {meta.Saldo:n2} | Transações: {meta.Transacoes.Count}");
    }

    Pausar("Fim da lista.");
}

void Depositar()
{
    Console.Clear();
    Console.WriteLine("=== Depositar ===");

    var meta = SelecionarMeta();
    if (meta is null) return;

    var valor = LerValor("Valor do depósito: ");
    if (valor is null) return;

    Console.Write("Descrição (opcional): ");
    var desc = Console.ReadLine();

    service.Depositar(meta, valor.Value, desc);
    Pausar("Depósito realizado com sucesso.");
}

void Sacar()
{
    Console.Clear();
    Console.WriteLine("=== Sacar ===");

    var meta = SelecionarMeta();
    if (meta is null) return;

    var valor = LerValor("Valor do saque: ");
    if (valor is null) return;

    Console.Write("Descrição (opcional): ");
    var desc = Console.ReadLine();

    service.Sacar(meta, valor.Value, desc);
    Pausar("Saque realizado com sucesso.");
}

void Extrato()
{
    Console.Clear();
    Console.WriteLine("=== Extrato ===");

    var meta = SelecionarMeta();
    if (meta is null) return;

    var extrato = service.GerarExtrato(meta);

    Console.WriteLine();
    Console.WriteLine(extrato);

    Pausar("Fim do extrato.");
}

Meta? SelecionarMeta()
{
    if (metas.Count == 0)
    {
        Pausar("Nenhuma meta cadastrada. Crie uma meta primeiro.");
        return null;
    }

    Console.WriteLine("Metas disponíveis:");
    foreach (var nome in metas.Keys.OrderBy(x => x))
        Console.WriteLine($"- {nome}");

    Console.WriteLine();
    Console.Write("Digite o nome da meta: ");
    var nomeMeta = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(nomeMeta))
    {
        Pausar("Nome da meta é obrigatório.");
        return null;
    }

    if (!metas.TryGetValue(nomeMeta, out var meta))
    {
        Pausar("Meta não encontrada.");
        return null;
    }

    return meta;
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
