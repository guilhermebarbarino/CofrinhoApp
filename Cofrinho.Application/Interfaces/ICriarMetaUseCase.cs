namespace Cofrinho.Application.Interfaces
{
    public interface ICriarMetaUseCase
    {
        Task ExecuteAsync(string nome, CancellationToken ct = default);
    }
}