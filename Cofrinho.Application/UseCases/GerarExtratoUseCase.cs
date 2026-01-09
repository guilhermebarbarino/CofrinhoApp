using Cofrinho.Application.Interfaces;
using Cofrinho.Domain.Enums;
using Cofrinho.Domain.Interfaces;
using Cofrinho.Domain.Utils;
using System.Globalization;
using System.Text;

namespace Cofrinho.Application.UseCases
{
    public class GerarExtratoUseCase : IGerarExtratoUseCase
    {
        private readonly IMetaRepository _repo;
        private static readonly CultureInfo PtBr = new("pt-BR");

        public GerarExtratoUseCase(IMetaRepository repo)
        {
            _repo = repo;
        }

        public Task<string> ExecuteAsync(string nomeMeta, CancellationToken ct = default)
        {
            var meta = _repo.GetByName(nomeMeta.Trim());
            if (meta is null)
                throw new KeyNotFoundException(ErrorMessages.MetaNaoEncontrada);

            var sb = new StringBuilder();
            sb.AppendLine($"Meta: {meta.Nome}");
            sb.AppendLine(new string('-', 40));

            foreach (var t in meta.Transacoes.OrderBy(x => x.Data))
            {
                var sinal = t.Tipo == TipoTransacao.Deposito ? "+" : "-";
                sb.AppendLine(
                    $"{t.Data:dd/MM/yyyy HH:mm} | {t.Tipo} | {sinal}{t.Valor.ToString("C2", PtBr)} | {t.Descricao}"
                );
            }

            sb.AppendLine(new string('-', 40));
            sb.AppendLine($"Saldo atual: {meta.Saldo.ToString("C2", PtBr)}");

            return Task.FromResult(sb.ToString());
        }
    }
}
