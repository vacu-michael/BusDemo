using Microsoft.Extensions.Logging;
using Models;
// Models.Commands and Models.Events do not exist as sub-namespaces; use direct types from Models
using MassTransit;
using System;
using System.Threading.Tasks;
using BLL;

namespace Worker.Consumers;

/// <summary>
/// Consumes StartWorkflowCommand, updates Application, and emits WorkflowCompleted event.
/// </summary>
public class StartWorkflowCommandConsumer(DemoBusinessLogic _bll, ILogger<StartWorkflowCommandConsumer> _logger) : IConsumer<StartWorkflow>
{
    /// <summary>
    /// Consumes the StartWorkflow command, updates Application, and emits WorkflowCompleted event.
    /// </summary>
    public async Task Consume(ConsumeContext<StartWorkflow> context)
    {
        var command = context.Message;
        var cancellationToken = context.CancellationToken;

        // Simulate general error
        var throwErrorSetting = await _bll.GetSettings(SettingsConstants.ThrowError);
        if (throwErrorSetting is not null && throwErrorSetting.Value is true)
        {
            _logger.LogError("Simulated error as per ThrowError setting. Failing StartWorkflowCommand for ApplicationId {ApplicationId}.", command.ApplicationId);
            await context.Publish(new WorkflowError(command.ApplicationId), cancellationToken);
            return;
        }

        // Simulate checking if the core system is available
        var coreIsAvailable = await _bll.GetSettings(SettingsConstants.CoreAvailable);
        if (coreIsAvailable is not null && coreIsAvailable.Value is false)
        {
            _logger.LogInformation("Core system is not available. Rescheduling StartWorkflowCommand ApplicationId {ApplicationId}.", command.ApplicationId);

            var rescheduleDate = DateTime.UtcNow.AddMinutes(5);

            await context.ScheduleSend(rescheduleDate, command, cancellationToken);
            _logger.LogDebug("Rescheduled StartWorkflowCommand for ApplicationId {ApplicationId} at {RescheduleDate}.", command.ApplicationId, rescheduleDate);

            var rescheduledEvent = new WorkflowRescheduled(command.ApplicationId, rescheduleDate);
            await context.Publish(rescheduledEvent, cancellationToken);
            _logger.LogDebug("WorkflowRescheduled event emitted for ApplicationId {ApplicationId} at {RescheduleDate}.", command.ApplicationId, rescheduleDate);
            return;
        }


        // Fetch the application
        var app = await _bll.GetApplication(command.ApplicationId);
        if (app is null)
        {
            _logger.LogWarning("Application with ID {ApplicationId} not found.", command.ApplicationId);
            return;
        }

        // Set a randomly generated 8-digit long as the AccountNumber
        var random = new Random();
        var randomValue = random.NextInt64(10_000_000, 99_000_000); // 8-digit number
        var accountNumber = randomValue + app.Id; // Do something with app.Id to have a reason to pull it from db

        await _bll.SetAccountNumberForApplication(app.Id, accountNumber);
        var last4 = randomValue.ToString()[^4..];
        _logger.LogInformation("Random AccountNumber xx{Last4} added to application {ApplicationId}.", last4, app.Id);

        var evt = new WorkflowCompleted(app.Id);
        await context.Publish(evt, cancellationToken);
        _logger.LogDebug("WorkflowCompleted event emitted for application {ApplicationId}.", app.Id);
    }
}
