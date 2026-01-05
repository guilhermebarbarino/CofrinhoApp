using Cofrinho.Console.Application.Interfaces;
using Cofrinho.Console.Domain.Entities;
using Cofrinho.Console.Domain.Interfaces;
using Cofrinho.Console.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Cofrinho.Console.Application.Services.UseCases
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
                throw new ArgumentException("Nome da meta é obrigatório.");

            nome = nome.Trim();

            if (_repo.Exists(nome))
                throw new InvalidOperationException("Já existe uma meta com esse nome.");

            _repo.Add(new Meta(nome));
            await _uow.SaveChangesAsync(ct);
            _logger.LogInformation("Meta criada com sucesso. Nome={Nome}", nome);
        }
    }
}