namespace Cofrinho.Application.Interfaces
{
    public interface ISacarUseCase
    {
        Task ExecuteAsync(string nomeMeta, decimal valor, string? descricao = null, CancellationToken ct = default);
    }
}