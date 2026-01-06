using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cofrinho.Infrastructure.Persistence;

public class CofrinhoDbContextFactory : IDesignTimeDbContextFactory<CofrinhoDbContext>
{
    public CofrinhoDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CofrinhoDbContext>();
        var dbPath = Path.Combine(AppContext.BaseDirectory, "cofrinho.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
        return new CofrinhoDbContext(optionsBuilder.Options);
    }
}