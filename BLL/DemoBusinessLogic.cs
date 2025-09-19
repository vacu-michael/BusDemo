
using DAL;
using Models;
using SAL;


namespace BLL;

public class DemoBusinessLogic(DemoDbContext db, BusService busService)
{
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

}
