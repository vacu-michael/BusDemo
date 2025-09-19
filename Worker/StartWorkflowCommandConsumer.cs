
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
// Models.Commands and Models.Events do not exist as sub-namespaces; use direct types from Models
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Worker;

/// <summary>
/// Consumes StartWorkflowCommand, updates Application, and emits WorkflowCompleted event.
/// </summary>
public class StartWorkflowCommandConsumer : IConsumer<StartWorkflow>
{
    private readonly DemoDbContext _dbContext;
    private readonly ILogger<StartWorkflowCommandConsumer> _logger;

    public StartWorkflowCommandConsumer(DemoDbContext dbContext, ILogger<StartWorkflowCommandConsumer> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Consumes the StartWorkflow command, updates Application, and emits WorkflowCompleted event.
    /// </summary>
    public async Task Consume(ConsumeContext<StartWorkflow> context)
    {
        var command = context.Message;
        var cancellationToken = context.CancellationToken;

        var app = await _dbContext.Applications.FirstOrDefaultAsync(a => a.Id == command.ApplicationId, cancellationToken);
        if (app is null)
        {
            _logger.LogWarning("Application with ID {ApplicationId} not found.", command.ApplicationId);
            return;
        }

        // Set a randomly generated long as the AccountNumber
        var random = new Random();
        var randomValue = ((long)random.Next() << 32) | (uint)random.Next();
        app.AccountNumber = randomValue;

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Random AccountNumber {RandomValue} added to application {ApplicationId}.", randomValue, app.Id);

        var evt = new WorkflowCompleted(app.Id);
        await context.Publish(evt, cancellationToken);
        _logger.LogInformation("WorkflowCompleted event emitted for application {ApplicationId}.", app.Id);
    }
}
