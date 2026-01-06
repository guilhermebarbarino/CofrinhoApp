namespace Cofrinho.Application.Interfaces
{
    public interface IGerarExtratoGeralUseCase
    {
        Task<string> ExecuteAsync(CancellationToken ct = default);
    }
}
