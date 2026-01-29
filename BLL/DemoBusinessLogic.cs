
using DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using SAL;

namespace BLL;

public sealed class DemoBusinessLogic : IDisposable
{
    private readonly DemoDbContext _db;
    private readonly BusService _busService;
    public event Action<Guid>? WorkflowCompleted;
    public event Action<Guid>? WorkflowRescheduled;
    public event Action<Guid>? WorkflowError;

    public DemoBusinessLogic(DemoDbContext db, BusService busService)
    {
        _db = db;
        _busService = busService;
        _busService.WorkflowCompleted += OnWorkflowCompleted;
        _busService.WorkflowRescheduled += OnWorkflowRescheduled;
        _busService.WorkflowError += OnWorkflowError;
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

        _ = _busService.PublishApplicationSubmittedEvent(correlationId, app.Id);
        return app;
    }

    public async Task<Application?> GetApplication(int id) =>
        await _db.Applications.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

    private void OnWorkflowCompleted(Guid correlationId) => WorkflowCompleted?.Invoke(correlationId);
    private void OnWorkflowRescheduled(Guid correlationId) => WorkflowRescheduled?.Invoke(correlationId);
    private void OnWorkflowError(Guid correlationId) => WorkflowError?.Invoke(correlationId);

    public void Dispose()
    {
        _busService.WorkflowCompleted -= OnWorkflowCompleted;
        _busService.WorkflowRescheduled -= OnWorkflowRescheduled;
        _busService.WorkflowError -= OnWorkflowError;
    }
}
