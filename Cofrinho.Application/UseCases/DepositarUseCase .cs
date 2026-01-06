using Cofrinho.Application.Interfaces;
using Cofrinho.Domain.Entities;
using Cofrinho.Domain.Enums;
using Cofrinho.Domain.Interfaces;
using Cofrinho.Domain.Utils;
using Cofrinho.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;


namespace Cofrinho.Application.UseCases
{
    public class DepositarUseCase : IDepositarUseCase
    {
        private readonly IMetaRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<DepositarUseCase> _logger;

        public DepositarUseCase(
            IMetaRepository repo,
            IUnitOfWork uow,
            ILogger<DepositarUseCase> logger)
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
                "Iniciando depósito. Meta={Meta} Valor={Valor}",
                nomeMeta, valor);

            if (string.IsNullOrWhiteSpace(nomeMeta))
            {
                _logger.LogWarning("Nome da meta não informado para depósito.");
                throw new ArgumentException(ErrorMessages.NomeMetaObrigatorio);
            }

            if (valor <= 0)
            {
                _logger.LogWarning(
                    "Valor inválido para depósito. Meta={Meta} Valor={Valor}",
                    nomeMeta, valor);

                throw new ArgumentException(ErrorMessages.TransacaoValorMenorQueZero);
            }

            var meta = _repo.GetByName(nomeMeta.Trim());

            if (meta is null)
            {
                _logger.LogWarning(
                    "Tentativa de depósito em meta inexistente. Meta={Meta}",
                    nomeMeta);

                throw new KeyNotFoundException(ErrorMessages.MetaNaoEncontrada);
            }

            meta.AdicionarTransacao(
                new Transacao(valor, TipoTransacao.Deposito, descricao));

            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Depósito realizado com sucesso. Meta={Meta} Valor={Valor} SaldoAtual={Saldo}",
                meta.Nome, valor, meta.Saldo);
        }
    }
}


