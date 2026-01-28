using Microsoft.Extensions.Logging;
using Models.Commands;
using Models.Events;
using MassTransit;
using System;
using System.Threading.Tasks;
using BLL;
using SAL;

namespace Worker.Consumers;

/// <summary>
/// Consumes ValidateName Command, updates Application, and emits a ValidateNameSucceeded event.
/// </summary>
public class ValidateNameConsumer(WorkerBusinessLogic bll, ILogger<ValidateNameConsumer> logger, EventService events) : IConsumer<ValidateName>
{
    /// <summary>
    /// Consumes the ValidateName command, updates Application, and emits ValidateNameSucceeded event.
    /// </summary>
    public async Task Consume(ConsumeContext<ValidateName> context)
    {
        var command = context.Message;
        logger.LogInformation("Validating name for CorrelationId {CorrelationId}.", command.CorrelationId);

        // Retrieve the application
        var app = await bll.GetApplication(command.CorrelationId)
            ?? throw new Exception("Unable to retrieve application from database");

        // Validate the name
        if (!bll.NameIsValid(app.Name))
        {
            _ = events.LogEventAsync("ValidateNameFailed", command.CorrelationId);
            logger.LogWarning("Validation failed for name on application with CorrelationId {CorrelationId}.", command.CorrelationId);

            // Throw to trigger a Fault<ValidateName> event
            throw new Exception("Name on application is banned");
        }

        _ = context.Publish(new ValidateNameSucceeded(command.CorrelationId));
        _ = events.LogEventAsync("ValidateNameSucceeded", command.CorrelationId);
        logger.LogInformation("Validation succeeded for name on application with CorrelationId {CorrelationId}.", command.CorrelationId);
    }
}
