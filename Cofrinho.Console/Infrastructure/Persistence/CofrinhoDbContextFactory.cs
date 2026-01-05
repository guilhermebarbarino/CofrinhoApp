using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cofrinho.Console.Infrastructure.Persistence;

public class CofrinhoDbContextFactory 
    : IDesignTimeDbContextFactory<CofrinhoDbContext>
{
    public CofrinhoDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CofrinhoDbContext>();

        optionsBuilder.UseSqlite("Data Source=cofrinho.db");

        return new CofrinhoDbContext(optionsBuilder.Options);
    }
}
