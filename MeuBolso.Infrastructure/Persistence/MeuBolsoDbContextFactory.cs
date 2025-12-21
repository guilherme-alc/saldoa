using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MeuBolso.Infrastructure.Persistence;

public class MeuBolsoDbContextFactory : IDesignTimeDbContextFactory<MeuBolsoDbContext>
{
    public MeuBolsoDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("POSTGRES_CONNECTION");
        
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Variável POSTGRES_CONNECTION não encontrada.");
        
        var optionsBuilder = new DbContextOptionsBuilder<MeuBolsoDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new MeuBolsoDbContext(optionsBuilder.Options);
    }
}