using Microsoft.Extensions.Logging;
using Models.Commands;
using Models.Events;
using MassTransit;
using System.Threading.Tasks;
using BLL;
using SAL;
using Models;

namespace Worker.Consumers;

/// <summary>
/// Consumes CreateAccount Command, updates Application, and emits a CreateAccountSucceeded event.
/// </summary>
public class CreateAccountConsumer(WorkerBusinessLogic bll, ILogger<CreateAccountConsumer> logger, EventService events) : IConsumer<CreateAccount>
{
    /// <summary>
    /// Consumes the CreateAccount command, updates Application, and emits CreateAccountSucceeded event.
    /// </summary>
    public async Task Consume(ConsumeContext<CreateAccount> context)
    {
        var command = context.Message;

        // Simulate checking if the core system is available
        var coreIsAvailable = await bll.GetSettings(SettingsConstants.CoreAvailable);

        if (coreIsAvailable is not null && coreIsAvailable.Value is false)
        {
            logger.LogInformation("Core system is not available. Rescheduling CreateAccountCommand CorrelationId {CorrelationId}.", command.CorrelationId);

            _ = events.LogEventAsync("CreateAccountDeferred", command.CorrelationId);
            _ = context.Publish(new DeferCreateAccount(command.CorrelationId, bll.GetDeferUntilDateTime()));
            return;
        }

        // Retrieve the application
        var app = await bll.GetApplication(command.CorrelationId);

        logger.LogInformation("Creating account for ApplicationId {ApplicationId}.", app.Id);

        await bll.SetAccountNumberForApplication(command.CorrelationId);

        _ = context.Publish(new CreateAccountSucceeded(command.CorrelationId));
        _ = events.LogEventAsync("CreateAccountSucceeded", command.CorrelationId);
    }
}
