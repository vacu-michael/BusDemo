using MassTransit;
using Models.Events;

namespace SAL;

/// <summary>
/// Service for sending commands via MassTransit bus.
/// </summary>
public class BusService(IBus bus)
{
    private readonly IBus _bus = bus;
    public event Action<Guid>? WorkflowCompleted;
    public event Action<Guid>? WorkflowRescheduled;
    public event Action<Guid>? WorkflowError;

    /// <summary>
    /// Sends a StartWorkflow command to the bus.
    /// </summary>
    /// <param name="applicationId">The application ID to start workflow for.</param>
    public Task PublishApplicationSubmittedEvent(Guid correlationId)
    {
        var command = new ApplicationSubmitted(correlationId);
        _bus.Publish(command);
        return Task.CompletedTask;
    }

    public void OnWorkflowCompleted(Guid correlationId) => WorkflowCompleted?.Invoke(correlationId);

    public void OnWorkflowRescheduled(Guid correlationId) => WorkflowRescheduled?.Invoke(correlationId);

    public void OnWorkflowError(Guid correlationId) => WorkflowError?.Invoke(correlationId);
}
