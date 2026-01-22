
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

    public async Task<Application> CreateApplicationAsync(string name, Guid correlationId)
    {
        var app = new Application
        {
            Name = name,
            CorrelationId = correlationId
        };
        _db.Applications.Add(app);
        await _db.SaveChangesAsync();
        return app;
    }

    public async Task SendStartWorkflowCommand(int applicationId, Guid correlationId)
    {
        await _busService.SendStartWorkflowCommand(applicationId, correlationId);
    }

    public async Task<Application?> GetApplication(int id)
    {
        return await _db.Applications.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
    }
}
