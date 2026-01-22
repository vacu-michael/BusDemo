using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DAL;

/// <summary>
/// Design-time factory for DemoDbContext to support EF Core tooling.
/// </summary>
public class DemoDbContextFactory : IDesignTimeDbContextFactory<DemoDbContext>
{
    public DemoDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<DemoDbContextFactory>()
            .Build();

        var connectionString = configuration["Db:ConnectionString"];

        var optionsBuilder = new DbContextOptionsBuilder<DemoDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new DemoDbContext(optionsBuilder.Options);
    }
}