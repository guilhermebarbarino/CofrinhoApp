using Cofrinho.Application.Interfaces;
using Cofrinho.Domain.Entities;
using Cofrinho.Domain.Interfaces;
using Cofrinho.Domain.Utils;
using Cofrinho.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Cofrinho.Application.UseCases
{
    public class CriarMetaUseCase : ICriarMetaUseCase
    {
        private readonly IMetaRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CriarMetaUseCase> _logger;

        public CriarMetaUseCase(IMetaRepository repo, IUnitOfWork uow, ILogger<CriarMetaUseCase> logger)
        {
            _repo = repo;
            _uow = uow;
            _logger = logger;
        }

        public async Task ExecuteAsync(string nome, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException(ErrorMessages.NomeMetaObrigatorio);

            nome = nome.Trim();

            if (_repo.Exists(nome))
                throw new InvalidOperationException("Já existe uma meta com esse nome.");

            _repo.Add(new Meta(nome));
            await _uow.SaveChangesAsync(ct);
            _logger.LogInformation("Meta criada com sucesso. Nome={Nome}", nome);
        }
    }
}