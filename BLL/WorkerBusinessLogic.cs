using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace BLL;

public sealed class WorkerBusinessLogic(DemoDbContext dbContext)
{
    public async Task<Application?> GetApplication(int id)
    {
        return await dbContext.Applications.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
    }
    public async Task SetAccountNumberForApplication(int applicationId, long accountNumber)
    {
        var existingApp = await dbContext.Applications.FirstOrDefaultAsync(a => a.Id == applicationId);
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