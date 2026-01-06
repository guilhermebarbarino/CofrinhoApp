using Cofrinho.Domain.Entities;

namespace Cofrinho.Application.Interfaces
{
    public interface IListarMetasUseCase
    {
        Task<IReadOnlyCollection<Meta>> ExecuteAsync(CancellationToken ct = default);
    }
}