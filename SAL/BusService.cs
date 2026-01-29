using MassTransit;
using Models.Events;

namespace SAL;

public class BusService(IBus bus)
{
    private readonly IBus _bus = bus;
    public event Action<Guid>? WorkflowCompleted;
    public event Action<Guid>? WorkflowRescheduled;
    public event Action<Guid>? WorkflowError;

    public Task PublishApplicationSubmittedEvent(Guid correlationId, int applicationId)
    {
        var command = new ApplicationSubmitted(correlationId, applicationId);
        _bus.Publish(command);
        return Task.CompletedTask;
    }

    public Task RetryWorkflow(Guid correlationId)
    {
        var command = new RetryRequested(correlationId);
        _bus.Publish(command);
        return Task.CompletedTask;
    }

    public Task OverrideNameValidation(Guid correlationId)
    {
        var command = new ValidateNameOverrideRequested(correlationId);
        _bus.Publish(command);
        return Task.CompletedTask;
    }

    public void OnWorkflowCompleted(Guid correlationId) => WorkflowCompleted?.Invoke(correlationId);

    public void OnWorkflowRescheduled(Guid correlationId) => WorkflowRescheduled?.Invoke(correlationId);

    public void OnWorkflowError(Guid correlationId) => WorkflowError?.Invoke(correlationId);
}
