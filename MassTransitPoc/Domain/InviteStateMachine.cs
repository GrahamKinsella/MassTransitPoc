using System.Diagnostics;
using MassTransit;
using MassTransit.Mediator;
using MassTransitPoc.Producers;
using MassTransitPoc.UseCases.CreateBrand;
using MassTransitPoc.UseCases.CreateUser;
using MassTransitPoc.UseCases.SendEmail;

namespace MassTransitPoc.Domain;

public class InviteStateMachine :
    MassTransitStateMachine<InviteState>
{
    //Need to add states for state machine
    public State BrandCreated { get; private set; }
    public State UserCreated { get; private set; }
    public State EmailSent { get; private set; }
    public State Complete { get; private set; }


    public InviteStateMachine(IMediator mediator)
    {
        // Tell the saga where to store the current state
        // when ints here theyre assigned values. In this case
        // 0 - None, 1 - Initial, 2 - Final, 3 - BrandCreated, 4 - UserCreated, 5 - EmailSent
        InstanceState(x => x.CurrentState);

        //On events that are in the Initial state, a new instance of the saga will be created. You can use the SetSagaFactory to control how the saga is instantiated.
        Event(() => InviteCreatedEvent,
            e => e.CorrelateById(cxt => cxt.Message.OperationId));
        Event(() => CreateBrandEvent,
            e => e.CorrelateById(cxt => cxt.Message.OperationId));
        Event(() => UserCreatedEvent,
            e => e.CorrelateById(cxt => cxt.Message.OperationId));
        Event(() => EmailSentEvent,
            e => e.CorrelateById(cxt => cxt.Message.OperationId));

        //Can't use a single event and multiple status. State machine has to react to events to change
        Initially(
            When(InviteCreatedEvent) // the event that should trigger this process flow
                .Then(context => { context.Saga.CorrelationId = context.Message.OperationId; })
                .Then(async context =>
                {
                    //use case will call service to do something
                   await mediator.Publish<CreateBrandRequest>(new { OperationId = context.Message.OperationId });
                })
                .Then(async context =>
                {
                    //produce event to change state of this machine. This will move onto next action
                    await mediator.Publish<InviteStateProducerRequest>(new
                        { OperationId = context.Message.OperationId, EventToProduce = nameof(BrandCreatedEvent) });
                })
                .TransitionTo(BrandCreated) //Your next state
                .Then(context =>
                    Debug.WriteLine("Invite Created and CreateBrandEvent Raised for {0}", context.Saga.CorrelationId)));

        During(BrandCreated, When(CreateBrandEvent)
            .Then(async context =>
            {
                await mediator.Publish<CreateUserRequest>(new { OperationId = context.Message.OperationId });
            })
            .Then(async context =>
            {
                await mediator.Publish<InviteStateProducerRequest>(new
                    { OperationId = context.Message.OperationId, EventToProduce = nameof(UserCreatedEvent) });
            })
            .TransitionTo(UserCreated)
            .Then(context =>
                Debug.WriteLine("User Created and UserCreatedEvent Raised for {0}", context.Saga.CorrelationId)));


        During(UserCreated, When(UserCreatedEvent)
            .Then(async context => { await mediator.Publish<SendEmailRequest>(new { OperationId = context.Message.OperationId }); })
            .Then(async context =>
            {
                await mediator.Publish<InviteStateProducerRequest>(new
                    { OperationId = context.Message.OperationId, EventToProduce = nameof(EmailSentEvent) });
            })
            .TransitionTo(EmailSent)
            .Then(context =>
                Debug.WriteLine("Email Sent and EmailSentEvent raisedReady: {0}", context.Saga.CorrelationId)));

        During(EmailSent, When(EmailSentEvent)
            .TransitionTo(Complete)
            .Then(context => Debug.WriteLine("Saga completed for: {0}", context.Saga.CorrelationId))
            .Finalize()); //sets saga to final state

        //completes and deletes saga from saga repository when state is final
        //SetCompletedWhenFinalized();
    }

    public Event<InviteCreatedEvent> InviteCreatedEvent { get; private set; }
    public Event<BrandCreatedEvent> CreateBrandEvent { get; private set; }
    public Event<UserCreatedEvent> UserCreatedEvent { get; private set; }
    public Event<EmailSentEvent> EmailSentEvent { get; private set; }
}