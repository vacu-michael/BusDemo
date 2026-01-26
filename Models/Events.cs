using MassTransit;

namespace Models.Events;

/// <summary>
/// Event raised when an application is submitted.
/// </summary>
public record ApplicationSubmitted(Guid CorrelationId) : CorrelatedBy<Guid>;

/// <summary>
/// Generic event to defer a command.
/// </summary>
public record Defer<T>(Guid CorrelationId, DateTime DeferUntil) : CorrelatedBy<Guid>;

/// <summary>
/// Event raised when ValidateName succeeds.
/// </summary>
public record ValidateNameSucceeded(Guid CorrelationId) : CorrelatedBy<Guid>;

/// <summary>
/// Event raised when CreateAccount succeeds.
/// </summary>
public record CreateAccountSucceeded(Guid CorrelationId) : CorrelatedBy<Guid>;

/// <summary>
/// Event raised when LinkAccount succeeds.
/// </summary>
public record LinkAccountSucceeded(Guid CorrelationId) : CorrelatedBy<Guid>;

/// <summary>
/// Event raised when a workflow is rescheduled.
/// </summary>
public record WorkflowRescheduled(Guid CorrelationId, DateTime NewRunAt) : CorrelatedBy<Guid>;

/// <summary>
/// Event raised when a workflow is completed.
/// </summary>
public record WorkflowCompleted(Guid CorrelationId) : CorrelatedBy<Guid>;

/// <summary>
/// Event raised when a workflow encounters an error.
/// </summary>
public record WorkflowError(Guid CorrelationId, string ErrorMessage) : CorrelatedBy<Guid>;

/// <summary>
/// Event raised when a command is deferred.
/// </summary>
public record CommandDeferred(Guid CorrelationId, string CommandName, DateTime DeferredUntil) : CorrelatedBy<Guid>;

/// <summary>
/// Event raised when a process completes.
/// </summary>
public record ProcessComplete(Guid CorrelationId) : CorrelatedBy<Guid>;

/// <summary>
/// Event raised when a process encounters an error.
/// </summary>
public record ProcessError(Guid CorrelationId, string ErrorMessage) : CorrelatedBy<Guid>;

/// <summary>
/// Generic event to resume a command or workflow step.
/// </summary>
public record Resume<T>(Guid CorrelationId) : CorrelatedBy<Guid>;

/// <summary>
/// Generic event to retry a command or workflow step.
/// </summary>
public record Retry<T>(Guid CorrelationId) : CorrelatedBy<Guid>;

