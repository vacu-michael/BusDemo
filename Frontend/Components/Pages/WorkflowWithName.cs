using System;

namespace Frontend.Components.Pages;

public record WorkflowWithName
{
    public required int ApplicationId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required string Name { get; init; }
    public required string CurrentState { get; init; }
    public required string LastErrorMessage { get; init; }
    public required DateTime LastUpdated { get; init; }
    public required DateTime CreatedAt { get; init; }
}
