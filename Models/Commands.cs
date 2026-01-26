using MassTransit;

namespace Models.Commands;

public record StartWorkflow(int ApplicationId, Guid CorrelationId);

/// <summary>
/// Command to validate a name.
/// </summary>
public record ValidateName(Guid CorrelationId) : CorrelatedBy<Guid>;

/// <summary>
/// Command to create an account.
/// </summary>
public record CreateAccount(Guid CorrelationId) : CorrelatedBy<Guid>;

/// <summary>
/// Command to get silly.
/// </summary>
public record LinkAccount(Guid CorrelationId) : CorrelatedBy<Guid>;