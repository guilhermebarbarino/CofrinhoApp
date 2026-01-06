namespace Cofrinho.Application.Interfaces
{
    public interface IGerarExtratoUseCase
    {
        Task<string> ExecuteAsync(string nomeMeta, CancellationToken ct = default);
    }
}