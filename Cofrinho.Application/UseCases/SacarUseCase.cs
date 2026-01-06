using Cofrinho.Application.Interfaces;
using Cofrinho.Domain.Entities;
using Cofrinho.Domain.Enums;
using Cofrinho.Domain.Interfaces;
using Cofrinho.Domain.Utils;
using Cofrinho.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Cofrinho.Application.UseCases
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
                throw new ArgumentException(ErrorMessages.NomeMetaObrigatorio);
            }

            if (valor <= 0)
            {
                _logger.LogWarning(
                    "Valor inválido para saque. Meta={Meta} Valor={Valor}",
                    nomeMeta, valor);

                throw new ArgumentException(ErrorMessages.TransacaoValorMenorQueZero);
            }

            var meta = _repo.GetByName(nomeMeta.Trim());

            if (meta is null)
            {
                _logger.LogWarning(
                    "Tentativa de saque em meta inexistente. Meta={Meta}",
                    nomeMeta);

                throw new KeyNotFoundException(ErrorMessages.MetaNaoEncontrada);
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
