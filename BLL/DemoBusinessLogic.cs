
using DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using SAL;

namespace BLL;

public sealed class DemoBusinessLogic
{
    private readonly DemoDbContext _db;
    private readonly BusService _busService;
    public event Action<int>? WorkflowCompleted;
    public event Action<int>? WorkflowRescheduled;
    public event Action<int>? WorkflowError;

    public DemoBusinessLogic(DemoDbContext db, BusService busService)
    {
        _db = db;
        _busService = busService;
        _busService.WorkflowCompleted += (appId) => WorkflowCompleted?.Invoke(appId);
        _busService.WorkflowRescheduled += (appId) => WorkflowRescheduled?.Invoke(appId);
        _busService.WorkflowError += (appId) => WorkflowError?.Invoke(appId);
    }

    public async Task<Application> CreateApplicationAsync()
    {
        var app = new Application();
        _db.Applications.Add(app);
        await _db.SaveChangesAsync();
        return app;
    }

    public async Task SendStartWorkflowCommand(int applicationId)
    {
        await _busService.SendStartWorkflowCommand(applicationId);
    }

    public async Task<Application?> GetApplication(int id)
    {
        return await _db.Applications.FindAsync(id);
    }

    public async Task SetAccountNumberForApplication(int applicationId, long accountNumber)
    {
        var existingApp = await _db.Applications.FindAsync(applicationId);
        if (existingApp == null) return;
        existingApp.AccountNumber = accountNumber;
        await _db.SaveChangesAsync();
        return;
    }

    public async Task<Settings?> GetSettings(string name)
    {
        return await _db.Settings.FirstOrDefaultAsync(s => s.Name == name);
    }
}
