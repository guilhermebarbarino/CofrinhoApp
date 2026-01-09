using Cofrinho.Domain.Entities;
using Cofrinho.Domain.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Cofrinho.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]

public class InMemoryMetaRepository : IMetaRepository
{
    private readonly Dictionary<string, Meta> _metas = new(StringComparer.OrdinalIgnoreCase);

    public void Add(Meta meta)
    {
        _metas[meta.Nome] = meta;
    }

    public Meta? GetByName(string nome)
    {
        _metas.TryGetValue(nome, out var meta);
        return meta;
    }

    public IReadOnlyCollection<Meta> GetAll()
    {
        return _metas.Values.OrderBy(m => m.Nome).ToList().AsReadOnly();
    }

    public bool Exists(string nome)
    {
        return _metas.ContainsKey(nome);
    }
}
