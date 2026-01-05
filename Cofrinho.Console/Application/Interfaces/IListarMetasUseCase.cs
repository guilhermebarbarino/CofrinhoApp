using Cofrinho.Console.Domain.Entities;

namespace Cofrinho.Console.Application.Interfaces
{
    public interface IListarMetasUseCase
    {
        Task<IReadOnlyCollection<Meta>> ExecuteAsync(CancellationToken ct = default);
    }
}