using Microsoft.EntityFrameworkCore;
using Models;

namespace DAL;

public class EventDbContext(DbContextOptions<EventDbContext> options) : DbContext(options)
{
    public DbSet<ApplicationEvent> ApplicationEvents => Set<ApplicationEvent>();
}
