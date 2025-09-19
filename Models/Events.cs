namespace Models;

public record WorkflowRescheduled(int ApplicationId, DateTime NewRunAt);
public record WorkflowCompleted(int ApplicationId);
public record WorkflowError(int ApplicationId);