using Cofrinho.Infrastructure.Persistence;
using System.Diagnostics.CodeAnalysis;

namespace Cofrinho.Infrastructure.UnitOfWork
{
    [ExcludeFromCodeCoverage]

    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly CofrinhoDbContext _ctx;

        public EfUnitOfWork(CofrinhoDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _ctx.SaveChangesAsync(ct);
    }
}