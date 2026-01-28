using MassTransit;
using Models.Events;
using SAL;

namespace Frontend.Consumers;

public class ProcessErrorConsumer(BusService busService, ILogger<ProcessErrorConsumer> logger) : IConsumer<ProcessErrored>
{
    public Task Consume(ConsumeContext<ProcessErrored> context)
    {
        var evt = context.Message;
        logger.LogInformation("ProcessErrored received for CorrelationId {CorrelationId}", evt.CorrelationId);
        busService.OnWorkflowError(evt.CorrelationId);
        return Task.CompletedTask;
    }
}
