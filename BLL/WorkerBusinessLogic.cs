using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace BLL;

public sealed class WorkerBusinessLogic(DemoDbContext dbContext)
{
    private readonly List<string> bannedNames = ["banned"];
    private readonly int deferDelaySeconds = 10;
    public async Task<Application> GetApplication(Guid correlationId) =>
        await dbContext.Applications.AsNoTracking().FirstOrDefaultAsync(a => a.CorrelationId == correlationId)
        ?? throw new Exception("Application not found");

    public async Task SetAccountNumberForApplication(Guid correlationId)
    {
        var existingApp = await dbContext.Applications.FirstOrDefaultAsync(a => a.CorrelationId == correlationId)
            ?? throw new Exception("Application not found");

        existingApp.AccountNumber = new Random().Next(100000, 999999);
        await dbContext.SaveChangesAsync();

        return;
    }

    public async Task<Settings?> GetSettings(string name) =>
        await dbContext.Settings.FirstOrDefaultAsync(s => s.Name == name);

    public bool NameIsValid(string name) =>
        !bannedNames.Contains(name, StringComparer.OrdinalIgnoreCase);

    public DateTime GetDeferUntilDateTime() =>
        DateTime.UtcNow.AddSeconds(deferDelaySeconds);
}