using Cofrinho.Domain.Entities;

namespace Cofrinho.Application.Interfaces;

public interface IObterMetaPorNomeUseCase
{
    Task<Meta> ExecuteAsync(string nome, CancellationToken ct = default);
}
