using Cofrinho.Console.Application.Interfaces;
using Cofrinho.Console.Domain.Entities;
using Cofrinho.Console.Domain.Interfaces;

namespace Cofrinho.Console.Application.Services.UseCases
{
    public class ListarMetasUseCase : IListarMetasUseCase
    {
        private readonly IMetaRepository _repo;

        public ListarMetasUseCase(IMetaRepository repo)
        {
            _repo = repo;
        }

        public Task<IReadOnlyCollection<Meta>> ExecuteAsync(CancellationToken ct = default)
            => Task.FromResult(_repo.GetAll());
    }
}