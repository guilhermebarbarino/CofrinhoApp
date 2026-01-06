using Cofrinho.Infrastructure.Persistence;

namespace Cofrinho.Infrastructure.UnitOfWork
{
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