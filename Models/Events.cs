using MassTransit;

namespace Models.Events;
// Scheduled message for resuming account creation (used only for scheduling)
public record ResumeCreateAccountScheduled(Guid CorrelationId) : CorrelatedBy<Guid>;

// Initial event to start the workflow
public record ApplicationSubmitted(Guid CorrelationId) : CorrelatedBy<Guid>;

// ValidateName Events
public record ValidateNameSucceeded(Guid CorrelationId) : CorrelatedBy<Guid>;
public record RetryValidateName(Guid CorrelationId) : CorrelatedBy<Guid>;

// CreateAccount Events
public record CreateAccountSucceeded(Guid CorrelationId) : CorrelatedBy<Guid>;
public record DeferCreateAccount(Guid CorrelationId, DateTime DeferUntil) : CorrelatedBy<Guid>;
public record ResumeCreateAccount(Guid CorrelationId) : CorrelatedBy<Guid>;
public record RetryCreateAccount(Guid CorrelationId) : CorrelatedBy<Guid>;

// LinkAccount Events
public record LinkAccountSucceeded(Guid CorrelationId) : CorrelatedBy<Guid>;
public record RetryLinkAccount(Guid CorrelationId) : CorrelatedBy<Guid>;

// General Workflow Events (for frontend consumption)
public record CommandDeferred(Guid CorrelationId, string CommandName, DateTime DeferredUntil) : CorrelatedBy<Guid>;
public record ProcessCompleted(Guid CorrelationId) : CorrelatedBy<Guid>;
public record ProcessErrored(Guid CorrelationId, string ErrorMessage) : CorrelatedBy<Guid>;
