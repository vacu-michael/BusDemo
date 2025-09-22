using MassTransit;
using Models;
using SAL;

namespace Frontend.Consumers;

public class WorkflowCompletedConsumer(BusService busService, ILogger<WorkflowCompletedConsumer> logger) : IConsumer<WorkflowCompleted>
{
    public Task Consume(ConsumeContext<WorkflowCompleted> context)
    {
        var evt = context.Message;
        logger.LogInformation("WorkflowCompleted received for ApplicationId {ApplicationId}", evt.ApplicationId);
        busService.OnWorkflowCompleted(evt.ApplicationId);
        return Task.CompletedTask;
    }
}
