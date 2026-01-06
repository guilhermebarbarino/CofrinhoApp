using Cofrinho.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cofrinho.Infrastructure.Persistence;

public class CofrinhoDbContext : DbContext
{
    public CofrinhoDbContext(DbContextOptions<CofrinhoDbContext> options) : base(options) { }

    public DbSet<Meta> Metas => Set<Meta>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CofrinhoDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
