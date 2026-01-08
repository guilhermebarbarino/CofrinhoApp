using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cofrinho.Infrastructure.Persistence;

public sealed class CofrinhoDbContextFactory : IDesignTimeDbContextFactory<CofrinhoDbContext>
{
    public CofrinhoDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CofrinhoDbContext>();

        var solutionDir = Directory.GetCurrentDirectory();
        var apiDir = Path.GetFullPath(Path.Combine(solutionDir, "Cofrinho.Api"));
        Directory.CreateDirectory(apiDir);

        var dbPath = Path.Combine(apiDir, "cofrinho.api.db");

        optionsBuilder.UseSqlite($"Data Source={dbPath}");
        return new CofrinhoDbContext(optionsBuilder.Options);
    }
}
