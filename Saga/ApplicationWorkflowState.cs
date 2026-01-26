using MassTransit;

namespace Saga;

public class OpenAccountState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = string.Empty;
    public int ApplicationId { get; set; }
    public Guid? ValidateNameTokenId { get; set; }
    public Guid? CreateAccountTokenId { get; set; }
    public Guid? LinkAccountTokenId { get; set; }
    public Guid? WorkflowCorrelationId { get; set; }
    public DateTime? LastUpdated { get; set; }
}