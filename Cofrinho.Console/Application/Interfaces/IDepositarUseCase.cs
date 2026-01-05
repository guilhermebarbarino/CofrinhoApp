namespace Cofrinho.Console.Application.Interfaces
{
    public interface IDepositarUseCase
    {
        Task ExecuteAsync(string nomeMeta, decimal valor, string? descricao = null, CancellationToken ct = default);
    }
}