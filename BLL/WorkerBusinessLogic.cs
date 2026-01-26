using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace BLL;

public sealed class WorkerBusinessLogic(DemoDbContext dbContext)
{
    public async Task<Application?> GetApplication(Guid correlationId)
    {
        return await dbContext.Applications.AsNoTracking().FirstOrDefaultAsync(a => a.CorrelationId == correlationId);
    }
    public async Task SetAccountNumberForApplication(Guid correlationId, long accountNumber)
    {
        var existingApp = await dbContext.Applications.FirstOrDefaultAsync(a => a.CorrelationId == correlationId);
        if (existingApp == null) return;
        existingApp.AccountNumber = accountNumber;
        await dbContext.SaveChangesAsync();
        return;
    }

    public async Task<Settings?> GetSettings(string name)
    {
        return await dbContext.Settings.FirstOrDefaultAsync(s => s.Name == name);
    }
}