using MassTransit;
using Models.Events;
using SAL;

namespace Frontend.Consumers;

public class ProcessRescheduledConsumer(BusService busService, ILogger<ProcessRescheduledConsumer> logger) : IConsumer<CommandDeferred>
{
    public Task Consume(ConsumeContext<CommandDeferred> context)
    {
        var evt = context.Message;
        logger.LogInformation("CommandDeferred received for CorrelationId {CorrelationId}", evt.CorrelationId);
        busService.OnWorkflowRescheduled(evt.CorrelationId);
        return Task.CompletedTask;
    }
}
