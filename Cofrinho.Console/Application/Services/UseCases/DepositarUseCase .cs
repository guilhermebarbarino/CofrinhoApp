using Cofrinho.Console.Application.Interfaces;
using Cofrinho.Console.Domain.Entities;
using Cofrinho.Console.Domain.Enums;
using Cofrinho.Console.Domain.Interfaces;
using Cofrinho.Console.Infrastructure.UnitOfWork;

namespace Cofrinho.Console.Application.Services.UseCases
{
    public class DepositarUseCase : IDepositarUseCase
    {
        private readonly IMetaRepository _repo;
        private readonly IUnitOfWork _uow;

        public DepositarUseCase(IMetaRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task ExecuteAsync(string nomeMeta, decimal valor, string? descricao = null, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(nomeMeta))
                throw new ArgumentException("Nome da meta é obrigatório.");

            var meta = _repo.GetByName(nomeMeta.Trim());
            if (meta is null)
                throw new KeyNotFoundException("Meta não encontrada.");

            meta.AdicionarTransacao(new Transacao(valor, TipoTransacao.Deposito, descricao));
            await _uow.SaveChangesAsync(ct);
        }
    }
}