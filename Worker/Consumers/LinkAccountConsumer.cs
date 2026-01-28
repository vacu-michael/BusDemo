using Microsoft.Extensions.Logging;
using Models.Commands;
using Models.Events;
using MassTransit;
using System.Threading.Tasks;
using BLL;
using SAL;
using Models;
using System;

namespace Worker.Consumers;

/// <summary>
/// Consumes LinkAccount Command, updates Application, and emits a LinkAccountSucceeded event.
/// </summary>
public class LinkAccountConsumer(WorkerBusinessLogic bll, ILogger<LinkAccountConsumer> logger, EventService events) : IConsumer<LinkAccount>
{
    /// <summary>
    /// Consumes the LinkAccount command, updates Application, and emits LinkAccountSucceeded event.
    /// </summary>
    public async Task Consume(ConsumeContext<LinkAccount> context)
    {
        var command = context.Message;
        logger.LogInformation("Linking account for CorrelationId {CorrelationId}.", command.CorrelationId);

        // Simulate general error
        var throwErrorSetting = await bll.GetSettings(SettingsConstants.ThrowError);
        if (throwErrorSetting is not null && throwErrorSetting.Value is true)
        {
            _ = events.LogEventAsync("WorkflowError", command.CorrelationId);
            logger.LogError("Simulated error as per ThrowError setting. Failing LinkAccount command for CorrelationId {CorrelationId}.", command.CorrelationId);
            throw new Exception("Simulated error as per ThrowError setting.");
        }

        // Here's where the main logic for linking an account would go
        // This step just here to illustrate the pattern


        await bll.SetAccountNumberForApplication(command.CorrelationId);

        _ = context.Publish(new LinkAccountSucceeded(command.CorrelationId));
        _ = events.LogEventAsync("LinkAccountSucceeded", command.CorrelationId);
        logger.LogInformation("Linking account succeeded for CorrelationId {CorrelationId}.", command.CorrelationId);
    }
}
