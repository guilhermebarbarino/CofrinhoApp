using Cofrinho.Console.Application.Interfaces;
using Cofrinho.Console.Domain.Entities;
using Cofrinho.Console.Domain.Enums;
using Cofrinho.Console.Domain.Interfaces;
using Cofrinho.Console.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Cofrinho.Console.Application.Services.UseCases
{
    public class SacarUseCase : ISacarUseCase
    {
        private readonly IMetaRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<SacarUseCase> _logger;

        public SacarUseCase(
            IMetaRepository repo,
            IUnitOfWork uow,
            ILogger<SacarUseCase> logger)
        {
            _repo = repo;
            _uow = uow;
            _logger = logger;
        }

        public async Task ExecuteAsync(
            string nomeMeta,
            decimal valor,
            string? descricao = null,
            CancellationToken ct = default)
        {
            _logger.LogInformation(
                "Iniciando saque. Meta={Meta} Valor={Valor}",
                nomeMeta, valor);

            if (string.IsNullOrWhiteSpace(nomeMeta))
            {
                _logger.LogWarning("Nome da meta não informado para saque.");
                throw new ArgumentException("Nome da meta é obrigatório.");
            }

            if (valor <= 0)
            {
                _logger.LogWarning(
                    "Valor inválido para saque. Meta={Meta} Valor={Valor}",
                    nomeMeta, valor);

                throw new ArgumentException("Valor deve ser maior que zero.");
            }

            var meta = _repo.GetByName(nomeMeta.Trim());

            if (meta is null)
            {
                _logger.LogWarning(
                    "Tentativa de saque em meta inexistente. Meta={Meta}",
                    nomeMeta);

                throw new KeyNotFoundException("Meta não encontrada.");
            }

            if (valor > meta.Saldo)
            {
                _logger.LogWarning(
                    "Saldo insuficiente para saque. Meta={Meta} Saldo={Saldo} ValorSolicitado={Valor}",
                    meta.Nome, meta.Saldo, valor);

                throw new InvalidOperationException("Saldo insuficiente.");
            }

            meta.AdicionarTransacao(
                new Transacao(valor, TipoTransacao.Saque, descricao));

            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Saque realizado com sucesso. Meta={Meta} Valor={Valor} SaldoAtual={Saldo}",
                meta.Nome, valor, meta.Saldo);
        }
    }
}
