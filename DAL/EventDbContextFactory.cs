using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DAL;

/// <summary>
/// Design-time factory for EventDbContext to support EF Core tooling.
/// </summary>
public class EventDbContextFactory : IDesignTimeDbContextFactory<EventDbContext>
{
    public EventDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<EventDbContextFactory>()
            .Build();

        var connectionString = configuration["Db:ConnectionString"];

        var optionsBuilder = new DbContextOptionsBuilder<EventDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new EventDbContext(optionsBuilder.Options);
    }
}
