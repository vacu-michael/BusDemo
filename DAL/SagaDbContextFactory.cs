using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DAL;

/// <summary>
/// Factory for creating SagaDbContext instances at design time for EF Core migrations.
/// </summary>
public class SagaDbContextFactory : IDesignTimeDbContextFactory<SagaDbContext>
{
    public SagaDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<SagaDbContextFactory>()
            .Build();

        var connectionString = configuration["Db:ConnectionString"];

        var optionsBuilder = new DbContextOptionsBuilder<SagaDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new SagaDbContext(optionsBuilder.Options);
    }
}
