using MassTransit;
using Models;

namespace SAL;

/// <summary>
/// Service for sending commands via MassTransit bus.
/// </summary>
public class BusService(IBus bus)
{
    private readonly IBus _bus = bus;
    public event Action<int>? WorkflowCompleted;

    /// <summary>
    /// Sends a StartWorkflow command to the bus.
    /// </summary>
    /// <param name="applicationId">The application ID to start workflow for.</param>
    public async Task SendStartWorkflowCommand(int applicationId)
    {
        var command = new StartWorkflow(applicationId);
        await _bus.Send(command);
    }

    public void OnWorkflowCompleted(int applicationId)
    {
        WorkflowCompleted?.Invoke(applicationId);
    }
}
