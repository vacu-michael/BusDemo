using MassTransit;
using Models;
using SAL;

namespace Frontend.Consumers;

public class WorkflowErrorConsumer(BusService busService, ILogger<WorkflowErrorConsumer> logger) : IConsumer<WorkflowError>
{
    public Task Consume(ConsumeContext<WorkflowError> context)
    {
        var evt = context.Message;
        logger.LogInformation("WorkflowError received for ApplicationId {ApplicationId}", evt.ApplicationId);
        busService.OnWorkflowError(evt.ApplicationId);
        return Task.CompletedTask;
    }
}
