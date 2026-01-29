using DAL;
using Saga;
using Microsoft.EntityFrameworkCore;
using Models;
using SAL;

namespace BLL;

public sealed class AdminBusinessLogic : IDisposable
{
    private readonly SagaDbContext _sagaDb;
    private readonly EventDbContext _eventsDb;
    private readonly DemoDbContext _demoDb;
    private readonly BusService _busService;
    public event Action? WorkflowUpdated;

    public AdminBusinessLogic(SagaDbContext sagaDb, EventDbContext eventsDb, DemoDbContext demoDb, BusService busService)
    {
        _sagaDb = sagaDb;
        _eventsDb = eventsDb;
        _demoDb = demoDb;
        _busService = busService;
        _busService.WorkflowCompleted += OnWorkflowUpdated;
        _busService.WorkflowRescheduled += OnWorkflowUpdated;
        _busService.WorkflowError += OnWorkflowUpdated;
    }

    public async Task<List<(OpenAccountState State, string? ApplicationName)>> GetAllOpenAccountStatesWithNames()
    {
        // Join OpenAccountState with Application by ApplicationId
        var states = await _sagaDb.ApplicationWorkflowStates.AsNoTracking().ToListAsync();
        var appIds = states.Select(s => s.ApplicationId).Distinct().ToList();

        // Use DemoDbContext to get application names
        var apps = await _demoDb.Applications
            .Where(a => appIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, a => a.Name);

        return [.. states.Select(s => (s, apps.TryGetValue(s.ApplicationId, out var name) ? name : null))];
    }

    public async Task<List<ApplicationEvent>> LoadEvents(Guid correlationId) =>
        await _eventsDb.ApplicationEvents
            .Where(e => e.CorrelationId == correlationId)
            .OrderBy(e => e.Timestamp)
            .ToListAsync();

    private void OnWorkflowUpdated(Guid correlationId) => WorkflowUpdated?.Invoke();

    public async Task RetryWorkflow(Guid applicationId) => await _busService.RetryWorkflow(applicationId);
    public async Task OverrideNameValidation(Guid applicationId) => await _busService.OverrideNameValidation(applicationId);

    public void Dispose()
    {
        _busService.WorkflowCompleted -= OnWorkflowUpdated;
        _busService.WorkflowRescheduled -= OnWorkflowUpdated;
        _busService.WorkflowError -= OnWorkflowUpdated;
    }
}