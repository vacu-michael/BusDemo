using Microsoft.Extensions.Logging;
using Models;
using Models.Commands;
using Models.Events;
using MassTransit;
using System;
using System.Threading.Tasks;
using BLL;
using SAL;

namespace Worker.Consumers;

/// <summary>
/// Consumes StartWorkflowCommand, updates Application, and emits WorkflowCompleted event.
/// </summary>
public class ValidateNameConsumer(WorkerBusinessLogic _bll, ILogger<StartWorkflowConsumer> _logger, EventService _events) : IConsumer<ValidateName>
{
    /// <summary>
    /// Consumes the StartWorkflow command, updates Application, and emits WorkflowCompleted event.
    /// </summary>
    public async Task Consume(ConsumeContext<ValidateName> context)
    {
        var command = context.Message;
        var cancellationToken = context.CancellationToken;

        // Simulate checking if the core system is available
        var coreIsAvailable = await _bll.GetSettings(SettingsConstants.CoreAvailable);

        if (coreIsAvailable is not null && coreIsAvailable.Value is false)
        {
            _logger.LogInformation("Core system is not available. Rescheduling StartWorkflowCommand CorrelationId {CorrelationId}.", command.CorrelationId);

            var rescheduleDate = DateTime.UtcNow.AddSeconds(5);

            var rescheduledEvent = new Defer<ValidateName>(command.CorrelationId, rescheduleDate);
            await context.Publish(rescheduledEvent, cancellationToken);

            _ = _events.LogEventAsync("ValidateNameRescheduled", command.CorrelationId);
            return;
        }

        // Fetch the application
        var app = await _bll.GetApplication(command.CorrelationId)
            ?? throw new Exception("Unable to retrieve application from database");

        if (string.Equals(app.Name, "banned", StringComparison.OrdinalIgnoreCase))
            throw new Exception("Name on application is banned");

        await context.Publish(new ValidateNameSucceeded(command.CorrelationId));
    }
}
