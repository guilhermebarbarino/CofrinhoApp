using Cofrinho.Application.Interfaces;
using Cofrinho.Domain.Entities;
using Cofrinho.Domain.Interfaces;

namespace Cofrinho.Application.UseCases
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