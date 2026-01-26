using MassTransit;
using Models.Events;
using Models.Commands;

namespace Saga;

public class ApplicationWorkflowStateMachine : MassTransitStateMachine<OpenAccountState>
{
    // Three states for each step: Waiting, Pending, Failed
    public State WaitingValidateName { get; private set; } = null!;
    public State PendingValidateName { get; private set; } = null!;
    public State FailedValidateName { get; private set; } = null!;

    public State WaitingCreateAccount { get; private set; } = null!;
    public State PendingCreateAccount { get; private set; } = null!;
    public State FailedCreateAccount { get; private set; } = null!;

    public State WaitingLinkAccount { get; private set; } = null!;
    public State PendingLinkAccount { get; private set; } = null!;
    public State FailedLinkAccount { get; private set; } = null!;

    public State Complete { get; private set; } = null!;

    // Initial Event to start the workflow
    public Event<ApplicationSubmitted> ApplicationSubmitted { get; private set; } = null!;

    // 5 Events for each command: Succeeded, Faulted, Deferred, Resume, Retry

    // ValidateName Events
    public Event<ValidateNameSucceeded> ValidateNameSucceeded { get; private set; } = null!;
    public Event<Fault<ValidateName>> ValidateNameFaulted { get; private set; } = null!;
    public Event<Defer<ValidateName>> ValidateNameDeferred { get; private set; } = null!;
    public Event<Resume<ValidateName>> ValidateNameResume { get; private set; } = null!;
    public Event<Retry<ValidateName>> ValidateNameRetry { get; private set; } = null!;

    // CreateAccount Events
    public Event<CreateAccountSucceeded> CreateAccountSucceeded { get; private set; } = null!;
    public Event<Fault<CreateAccount>> CreateAccountFaulted { get; private set; } = null!;
    public Event<Defer<CreateAccount>> CreateAccountDeferred { get; private set; } = null!;
    public Event<Resume<CreateAccount>> CreateAccountResume { get; private set; } = null!;
    public Event<Retry<CreateAccount>> CreateAccountRetry { get; private set; } = null!;

    // LinkAccount Events
    public Event<LinkAccountSucceeded> LinkAccountSucceeded { get; private set; } = null!;
    public Event<Fault<LinkAccount>> LinkAccountFaulted { get; private set; } = null!;
    public Event<Defer<LinkAccount>> LinkAccountDeferred { get; private set; } = null!;
    public Event<Resume<LinkAccount>> LinkAccountResume { get; private set; } = null!;
    public Event<Retry<LinkAccount>> LinkAccountRetry { get; private set; } = null!;

    // Schedules for publishing events on a delay
    public Schedule<OpenAccountState, Resume<ValidateName>> ValidateNameSchedule { get; private set; } = null!;
    public Schedule<OpenAccountState, Resume<CreateAccount>> CreateAccountSchedule { get; private set; } = null!;
    public Schedule<OpenAccountState, Resume<LinkAccount>> LinkAccountSchedule { get; private set; } = null!;

    public ApplicationWorkflowStateMachine()
    {
        Schedule(() => ValidateNameSchedule, x => x.ValidateNameTokenId, s =>
       {
           s.Received = e => e.CorrelateById(context => context.Message.CorrelationId);
       });

        Schedule(() => CreateAccountSchedule, x => x.CreateAccountTokenId, s =>
        {
            s.Received = e => e.CorrelateById(context => context.Message.CorrelationId);
        });

        Schedule(() => LinkAccountSchedule, x => x.LinkAccountTokenId, s =>
        {
            s.Received = e => e.CorrelateById(context => context.Message.CorrelationId);
        });

        InstanceState(x => x.CurrentState);

        // Initial transition
        Initially(
            When(ApplicationSubmitted)
                .Then(context =>
                {
                    context.Saga.WorkflowCorrelationId = context.Message.CorrelationId;
                    context.Saga.LastUpdated = DateTime.UtcNow;
                })
                .TransitionTo(WaitingValidateName)
                .Publish(context => new ValidateName(context.Message.CorrelationId))
        );

        // ValidateName transitions
        During(PendingValidateName,
            When(ValidateNameSucceeded)
                .Then(context => context.Saga.LastUpdated = DateTime.UtcNow)
                .TransitionTo(PendingCreateAccount)
                .Publish(context => new CreateAccount(context.Message.CorrelationId)),

            When(ValidateNameFaulted)
                .Then(context => context.Saga.LastUpdated = DateTime.UtcNow)
                .TransitionTo(FailedValidateName)
                .Publish(context => new ProcessError(context.Message.Message.CorrelationId, context.Message.Exceptions[0].Message)),

            When(ValidateNameDeferred)
                .Then(context => context.Saga.LastUpdated = DateTime.UtcNow)
                .TransitionTo(WaitingValidateName)
                .Schedule(ValidateNameSchedule, context => new Resume<ValidateName>(context.Message.CorrelationId), context => TimeSpan.FromSeconds(10))
                .Publish(context => new CommandDeferred(context.Message.CorrelationId, nameof(ValidateName), DateTime.UtcNow.AddSeconds(10)))
        );

        During(WaitingValidateName,
            When(ValidateNameResume)
                .Then(context => context.Saga.LastUpdated = DateTime.UtcNow)
                .TransitionTo(PendingValidateName)
                .Publish(context => new ValidateName(context.Message.CorrelationId))
        );

        During(FailedValidateName,
            When(ValidateNameRetry)
                .Then(context => context.Saga.LastUpdated = DateTime.UtcNow)
                .TransitionTo(PendingValidateName)
                .Publish(context => new ValidateName(context.Message.CorrelationId))
        );

        // CreateAccount transitions
        During(PendingCreateAccount,
            When(CreateAccountSucceeded)
                .Then(context => context.Saga.LastUpdated = DateTime.UtcNow)
                .TransitionTo(PendingLinkAccount)
                .Publish(context => new LinkAccount(context.Message.CorrelationId)),

            When(CreateAccountFaulted)
                .Then(context => context.Saga.LastUpdated = DateTime.UtcNow)
                .TransitionTo(FailedCreateAccount)
                .Publish(context => new ProcessError(context.Message.Message.CorrelationId, context.Message.Exceptions[0].Message)),

            When(CreateAccountDeferred)
                .Then(context => context.Saga.LastUpdated = DateTime.UtcNow)
                .TransitionTo(WaitingCreateAccount)
                .Schedule(CreateAccountSchedule, context => new Resume<CreateAccount>(context.Message.CorrelationId), context => TimeSpan.FromSeconds(10))
                .Publish(context => new CommandDeferred(context.Message.CorrelationId, nameof(CreateAccount), DateTime.UtcNow.AddSeconds(10)))
        );

        During(WaitingCreateAccount,
            When(CreateAccountResume)
                .Then(context => context.Saga.LastUpdated = DateTime.UtcNow)
                .TransitionTo(PendingCreateAccount)
                .Publish(context => new CreateAccount(context.Message.CorrelationId))
        );

        During(FailedCreateAccount,
            When(CreateAccountRetry)
                .Then(context => context.Saga.LastUpdated = DateTime.UtcNow)
                .TransitionTo(PendingCreateAccount)
                .Publish(context => new CreateAccount(context.Message.CorrelationId))
        );

        // LinkAccount transitions
        During(PendingLinkAccount,
            When(LinkAccountSucceeded)
                .Then(context => context.Saga.LastUpdated = DateTime.UtcNow)
                .TransitionTo(Complete)
                .Publish(context => new WorkflowCompleted(context.Message.CorrelationId)),

            When(LinkAccountFaulted)
                .Then(context => context.Saga.LastUpdated = DateTime.UtcNow)
                .TransitionTo(FailedLinkAccount)
                .Publish(context => new ProcessError(context.Message.Message.CorrelationId, context.Message.Exceptions[0].Message)),

            When(LinkAccountDeferred)
                .Then(context => context.Saga.LastUpdated = DateTime.UtcNow)
                .TransitionTo(WaitingLinkAccount)
                .Schedule(LinkAccountSchedule, context => new Resume<LinkAccount>(context.Message.CorrelationId), context => TimeSpan.FromSeconds(10))
                .Publish(context => new CommandDeferred(context.Message.CorrelationId, nameof(LinkAccount), DateTime.UtcNow.AddSeconds(10)))
        );

        During(WaitingLinkAccount,
            When(LinkAccountResume)
                .Then(context => context.Saga.LastUpdated = DateTime.UtcNow)
                .TransitionTo(PendingLinkAccount)
                .Publish(context => new LinkAccount(context.Message.CorrelationId))
        );

        During(FailedLinkAccount,
            When(LinkAccountRetry)
                .Then(context => context.Saga.LastUpdated = DateTime.UtcNow)
                .TransitionTo(PendingLinkAccount)
                .Publish(context => new LinkAccount(context.Message.CorrelationId))
        );
    }
}
