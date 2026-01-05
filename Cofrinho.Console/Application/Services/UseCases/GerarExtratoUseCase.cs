using Cofrinho.Console.Application.Interfaces;
using Cofrinho.Console.Domain.Interfaces;
using System.Text;

namespace Cofrinho.Console.Application.Services.UseCases
{
    public class GerarExtratoUseCase : IGerarExtratoUseCase
    {
        private readonly IMetaRepository _repo;

        public GerarExtratoUseCase(IMetaRepository repo)
        {
            _repo = repo;
        }

        public Task<string> ExecuteAsync(string nomeMeta, CancellationToken ct = default)
        {
            var meta = _repo.GetByName(nomeMeta.Trim());
            if (meta is null)
                throw new KeyNotFoundException("Meta não encontrada.");

            var sb = new StringBuilder();
            sb.AppendLine($"Meta: {meta.Nome}");
            sb.AppendLine(new string('-', 40));

            foreach (var t in meta.Transacoes.OrderBy(x => x.Data))
            {
                var sinal = t.Tipo.ToString() == "Deposito" ? "+" : "-";
                sb.AppendLine($"{t.Data:dd/MM/yyyy HH:mm} | {t.Tipo} | {sinal}R$ {t.Valor:n2} | {t.Descricao}");
            }

            sb.AppendLine(new string('-', 40));
            sb.AppendLine($"Saldo atual: R$ {meta.Saldo:n2}");

            return Task.FromResult(sb.ToString());
        }
    }
}