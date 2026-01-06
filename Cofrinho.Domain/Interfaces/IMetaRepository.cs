using Cofrinho.Domain.Entities;

namespace Cofrinho.Domain.Interfaces;

public interface IMetaRepository
{
    void Add(Meta meta);
    Meta? GetByName(string nome);
    IReadOnlyCollection<Meta> GetAll();
    bool Exists(string nome);
}
