using MassTransit;
using Models.Events;
using Models.Commands;

namespace Saga;

public class ApplicationWorkflowStateMachine : MassTransitStateMachine<OpenAccountState>
{
    // Up to three states for each step: Waiting, Pending, Failed
    public State PendingValidateName { get; private set; } = null!;
    public State FailedValidateName { get; private set; } = null!;

    public State WaitingCreateAccount { get; private set; } = null!;
    public State PendingCreateAccount { get; private set; } = null!;
    public State FailedCreateAccount { get; private set; } = null!;

    public State PendingLinkAccount { get; private set; } = null!;
    public State FailedLinkAccount { get; private set; } = null!;

    public State Complete { get; private set; } = null!;

    // Initial Event to start the workflow
    public Event<ApplicationSubmitted> ApplicationSubmitted { get; private set; } = null!;


    // ValidateName Events
    public Event<ValidateNameSucceeded> ValidateNameSucceeded { get; private set; } = null!;
    public Event<Fault<ValidateName>> ValidateNameFaulted { get; private set; } = null!;
    public Event<ValidateNameOverrideRequested> ValidateNameOverrideRequested { get; private set; } = null!;

    // CreateAccount Events
    public Event<CreateAccountSucceeded> CreateAccountSucceeded { get; private set; } = null!;
    public Event<Fault<CreateAccount>> CreateAccountFaulted { get; private set; } = null!;
    public Event<DeferCreateAccount> CreateAccountDeferred { get; private set; } = null!;
    public Event<ResumeCreateAccount> CreateAccountResume { get; private set; } = null!;
    public Schedule<OpenAccountState, ResumeCreateAccountScheduled> CreateAccountSchedule { get; private set; } = null!;

    // LinkAccount Events
    public Event<LinkAccountSucceeded> LinkAccountSucceeded { get; private set; } = null!;
    public Event<Fault<LinkAccount>> LinkAccountFaulted { get; private set; } = null!;

    // Shared Retry Event
    public Event<RetryRequested> RetryRequested { get; private set; } = null!;

    public ApplicationWorkflowStateMachine()
    {
        Schedule(() => CreateAccountSchedule, x => x.CreateAccountTokenId, s =>
        {
            s.Received = r => r.CorrelateById(context => context.Message.CorrelationId);
        });

        InstanceState(x => x.CurrentState);

        // Initial transition
        Initially(
            When(ApplicationSubmitted)
                .Then(SetApplicationId)
                .Then(UpdateCreatedAt)
                .Then(UpdateLastUpdated)
                .TransitionTo(PendingValidateName)
                .Publish(context => new ValidateName(context.Message.CorrelationId))
        );

        // ValidateName transitions
        During(PendingValidateName,
            When(ValidateNameSucceeded)
                .Then(UpdateLastUpdated)
                .TransitionTo(PendingCreateAccount)
                .Publish(context => new CreateAccount(context.Message.CorrelationId)),

            When(ValidateNameFaulted)
                .Then(UpdateLastError)
                .TransitionTo(FailedValidateName)
                .Publish(context => new ProcessErrored(context.Message.Message.CorrelationId, context.Message.Exceptions[0].Message))
        );

        During(FailedValidateName,
            When(RetryRequested)
                .Then(UpdateLastUpdated)
                .TransitionTo(PendingValidateName)
                .Publish(context => new ValidateName(context.Message.CorrelationId)),

            When(ValidateNameOverrideRequested)
                .Then(UpdateLastUpdated)
                .TransitionTo(PendingCreateAccount)
                .Publish(context => new CreateAccount(context.Message.CorrelationId))
        );

        // CreateAccount transitions
        During(PendingCreateAccount,
            When(CreateAccountSucceeded)
                .Then(UpdateLastUpdated)
                .TransitionTo(PendingLinkAccount)
                .Publish(context => new LinkAccount(context.Message.CorrelationId)),

            When(CreateAccountFaulted)
                .Then(UpdateLastError)
                .TransitionTo(FailedCreateAccount)
                .Publish(context => new ProcessErrored(context.Message.Message.CorrelationId, context.Message.Exceptions[0].Message)),

            When(CreateAccountDeferred)
                .Then(UpdateLastUpdated)
                .TransitionTo(WaitingCreateAccount)
                .Schedule(CreateAccountSchedule, context => new ResumeCreateAccountScheduled(context.Message.CorrelationId), context => context.Message.DeferUntil)
                .Publish(context => new CommandDeferred(context.Message.CorrelationId, nameof(CreateAccount), DateTime.UtcNow.AddSeconds(10)))
        );

        During(WaitingCreateAccount,
            When(CreateAccountSchedule.Received)
                .Then(UpdateLastUpdated)
                .Publish(context => new ResumeCreateAccount(context.Message.CorrelationId))
        );

        During(WaitingCreateAccount,
            When(CreateAccountResume)
                .Then(UpdateLastUpdated)
                .TransitionTo(PendingCreateAccount)
                .Publish(context => new CreateAccount(context.Message.CorrelationId))
        );

        During(FailedCreateAccount,
            When(RetryRequested)
                .Then(UpdateLastUpdated)
                .TransitionTo(PendingCreateAccount)
                .Publish(context => new CreateAccount(context.Message.CorrelationId))
        );

        // LinkAccount transitions
        During(PendingLinkAccount,
            When(LinkAccountSucceeded)
                .Then(UpdateLastUpdated)
                .TransitionTo(Complete)
                .Publish(context => new ProcessCompleted(context.Message.CorrelationId)),

            When(LinkAccountFaulted)
                .Then(UpdateLastError)
                .TransitionTo(FailedLinkAccount)
                .Publish(context => new ProcessErrored(context.Message.Message.CorrelationId, context.Message.Exceptions[0].Message))
        );

        During(FailedLinkAccount,
            When(RetryRequested)
                .Then(UpdateLastUpdated)
                .TransitionTo(PendingLinkAccount)
                .Publish(context => new LinkAccount(context.Message.CorrelationId))
        );
    }

    static void SetApplicationId(BehaviorContext<OpenAccountState, ApplicationSubmitted> context)
        => context.Saga.ApplicationId = context.Message.ApplicationId;

    static void UpdateCreatedAt(BehaviorContext<OpenAccountState> context)
        => context.Saga.CreatedAt = DateTime.UtcNow;

    static void UpdateLastUpdated(BehaviorContext<OpenAccountState> context)
        => context.Saga.LastUpdated = DateTime.UtcNow;

    static void UpdateLastError<T>(BehaviorContext<OpenAccountState, Fault<T>> context)
    {
        context.Saga.LastUpdated = DateTime.UtcNow;
        context.Saga.LastErrorMessage = context.Message.Exceptions.FirstOrDefault()?.Message ?? "Unknown error";
    }
}
