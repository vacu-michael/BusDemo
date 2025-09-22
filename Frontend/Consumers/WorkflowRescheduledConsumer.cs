using MassTransit;
using Models;
using SAL;

namespace Frontend.Consumers;

public class WorkflowRescheduledConsumer(BusService busService, ILogger<WorkflowRescheduledConsumer> logger) : IConsumer<WorkflowRescheduled>
{
    public Task Consume(ConsumeContext<WorkflowRescheduled> context)
    {
        var evt = context.Message;
        logger.LogInformation("WorkflowRescheduled received for ApplicationId {ApplicationId}", evt.ApplicationId);
        busService.OnWorkflowRescheduled(evt.ApplicationId);
        return Task.CompletedTask;
    }
}
