using Microsoft.EntityFrameworkCore;
using Saga;

namespace DAL;

public class SagaDbContext(DbContextOptions<SagaDbContext> options) : DbContext(options)
{
    public DbSet<OpenAccountState> ApplicationWorkflowStates => Set<OpenAccountState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OpenAccountState>().HasKey(x => x.CorrelationId);
        base.OnModelCreating(modelBuilder);
    }
}
