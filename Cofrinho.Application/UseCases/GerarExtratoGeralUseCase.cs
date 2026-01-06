using Cofrinho.Application.Interfaces;
using Cofrinho.Domain.Enums;
using Cofrinho.Domain.Interfaces;
using System.Text;

namespace Cofrinho.Console.Application.Services.UseCases;

public sealed class GerarExtratoGeralUseCase : IGerarExtratoGeralUseCase
{
    private readonly IMetaRepository _repo;

    public GerarExtratoGeralUseCase(IMetaRepository repo)
    {
        _repo = repo;
    }

    public async Task<string> ExecuteAsync(CancellationToken ct = default)
    {
        
        var metas = _repo.GetAll();

        if (metas is null || metas.Count == 0)
            return "Nenhuma meta cadastrada.";

        var sb = new StringBuilder();
        sb.AppendLine("=== EXTRATO GERAL ===");
        sb.AppendLine($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}");
        sb.AppendLine();

        foreach (var meta in metas.OrderBy(m => m.Nome))
        {
            sb.AppendLine($"--- {meta.Nome} ---");
            sb.AppendLine($"Saldo: R$ {meta.Saldo:n2}");

            if (meta.Transacoes.Count == 0)
            {
                sb.AppendLine("Sem transações.");
                sb.AppendLine();
                continue;
            }

            foreach (var t in meta.Transacoes.OrderBy(x => x.Data))
            {
                var tipo = t.Tipo == TipoTransacao.Deposito ? "DEP" : "SAQ";
                var sinal = t.Tipo.ToString() == "Deposito" ? "+" : "-";
                sb.AppendLine($"{t.Data:dd/MM HH:mm} | {tipo} | {sinal}R$ {t.Valor:n2} | {t.Descricao}");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
