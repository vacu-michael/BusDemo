using MassTransit;
using Models.Events;
using SAL;

namespace Frontend.Consumers;

public class ProcessCompletedConsumer(BusService busService, ILogger<ProcessCompletedConsumer> logger) : IConsumer<ProcessCompleted>
{
    public Task Consume(ConsumeContext<ProcessCompleted> context)
    {
        var evt = context.Message;
        logger.LogInformation("ProcessCompleted received for CorrelationId {CorrelationId}", evt.CorrelationId);
        busService.OnWorkflowCompleted(evt.CorrelationId);
        return Task.CompletedTask;
    }
}
