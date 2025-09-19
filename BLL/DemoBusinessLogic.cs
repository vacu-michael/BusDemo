
using DAL;
using Models;
using SAL;

namespace BLL;

public sealed class DemoBusinessLogic : IDisposable
{
    private readonly DemoDbContext db;
    private readonly BusService busService;
    public event Action<int>? WorkflowCompleted;

    public DemoBusinessLogic(DemoDbContext db, BusService busService)
    {
        this.db = db;
        this.busService = busService;
        busService.WorkflowCompleted += OnWorkflowCompleted;
    }

    private void OnWorkflowCompleted(int applicationId)
    {
        WorkflowCompleted?.Invoke(applicationId);
    }

    public async Task<Application> CreateApplicationAsync()
    {
        var app = new Application();
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        return app;
    }

    /// <summary>
    /// Sends a StartWorkflow command for the given application ID.
    /// </summary>
    public async Task SendStartWorkflowCommand(int applicationId)
    {
        await busService.SendStartWorkflowCommand(applicationId);
    }

    public Application? GetApplication(int id)
    {
        return db.Applications.Find(id);
    }

    public Application? UpdateApplication(Application app)
    {
        var existingApp = db.Applications.Find(app.Id);
        if (existingApp == null) return null;
        existingApp.AccountNumber = app.AccountNumber;
        db.SaveChanges();
        return existingApp;
    }

    public Settings? GetSettings(string name)
    {
        return db.Settings.FirstOrDefault(s => s.Name == name);
    }

    public void Dispose()
    {
        busService.WorkflowCompleted -= OnWorkflowCompleted;
    }
}
