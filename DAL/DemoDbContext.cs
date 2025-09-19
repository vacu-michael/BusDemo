using Microsoft.EntityFrameworkCore;
using Models;

namespace DAL;

public class DemoDbContext(DbContextOptions<DemoDbContext> options) : DbContext(options)
{
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Settings> Settings => Set<Settings>();
}
