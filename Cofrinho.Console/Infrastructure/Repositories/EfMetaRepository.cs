using Cofrinho.Console.Domain.Entities;
using Cofrinho.Console.Domain.Interfaces;
using Cofrinho.Console.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cofrinho.Console.Infrastructure.Repositories;

public class EfMetaRepository : IMetaRepository
{
    private readonly CofrinhoDbContext _ctx;

    public EfMetaRepository(CofrinhoDbContext ctx)
    {
        _ctx = ctx;
    }

    public void Add(Meta meta)
    {
        _ctx.Metas.Add(meta);
        _ctx.SaveChanges();
    }

    public Meta? GetByName(string nome)
    {
        return _ctx.Metas
            .Include("_transacoes")
            .FirstOrDefault(m => m.Nome == nome);
    }

    public IReadOnlyCollection<Meta> GetAll()
    {
        return _ctx.Metas
            .Include("_transacoes")
            .OrderBy(m => m.Nome)
            .ToList()
            .AsReadOnly();
    }

    public bool Exists(string nome)
    {
        return _ctx.Metas.Any(m => m.Nome == nome);
    }
}
