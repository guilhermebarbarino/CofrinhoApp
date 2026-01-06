using Cofrinho.Domain.Entities;

namespace Cofrinho.Application.Interfaces;

public interface ICofrinhoAppService
{
    Meta CriarMeta(string nome);
    void Depositar(string nomeMeta, decimal valor, string? descricao = null);
    void Sacar(string nomeMeta, decimal valor, string? descricao = null);
    string GerarExtrato(string nomeMeta);
    IReadOnlyCollection<Meta> ListarMetas();
}
