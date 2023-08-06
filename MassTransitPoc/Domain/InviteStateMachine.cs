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
    public State CreateBrand { get; private set; }
    public State CreateUser { get; private set; }
    public State SendEmail { get; private set; }
    public State Complete { get; private set; }
    public State Failed { get; private set; }


    public InviteStateMachine(IMediator mediator)
    {
        // Tell the saga where to store the current state
        // when ints here theyre assigned values. In this case
        // 0 - None, 1 - Initial, 2 - Final, 3 - BrandCreated, 4 - UserCreated, 5 - EmailSent
        InstanceState(x => x.CurrentState);

        //On events that are in the Initial state, a new instance of the saga will be created. You can use the SetSagaFactory to control how the saga is instantiated.
        Event(() => InviteUpdatedEvent,
            e => e.CorrelateById(cxt => cxt.Message.OperationId));
        Event(() => InviteFailedEvent,
            e => e.CorrelateById(cxt => cxt.Message.OperationId));

        //Can't use a single event and multiple status. State machine has to react to events to change
        Initially(
            When(InviteUpdatedEvent) // the event that should trigger this process flow
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.OperationId;
                    context.Saga.Email = context.Message.Email;
                    context.Saga.Comments = context.Message.Comments;
                    context.Saga.BrandName = context.Message.BrandName;
                    context.Saga.IsPlanSupported = context.Message.IsPlanSupported;
                    context.Saga.OrganisationId = context.Message.OrganisationId;
                    context.Saga.Plan = context.Message.Plan;
                    context.Saga.Region = context.Message.Region;
                    context.Saga.PartnerId = context.Message.PartnerId;
                    context.Saga.Variant = context.Message.Variant;
                })
                .Then(async context =>
                {
                    //produce event to change state of this machine. This will move onto next action
                    await mediator.Publish<InviteStateProducerRequest>(new
                    {
                        OperationId = context.Message.OperationId, 
                        Status = "CreateBrand",
                        Plan = context.Saga.Plan,
                        Region = context.Saga.Region,
                        Email = context.Saga.Email,



                    });
                })
                .TransitionTo(CreateBrand) //Your next state
                .Then(context =>
                    Debug.WriteLine("Brand creation requested for saga {0}", context.Saga.CorrelationId)));


        During(CreateBrand, When(InviteUpdatedEvent)
            .Then(async context =>
            {
                await mediator.Publish<InviteStateProducerRequest>(new
                    { OperationId = context.Message.OperationId, Status = "CreateUser" });
            })
            .TransitionTo(CreateUser)
            .Then(context =>
                Debug.WriteLine("User creation requested for saga {0}", context.Saga.CorrelationId)));

        During(CreateUser, When(InviteUpdatedEvent)
            .Then(async context =>
            {
                await mediator.Publish<InviteStateProducerRequest>(new
                { OperationId = context.Message.OperationId, Status = "SendEmail" });
            })
            .TransitionTo(SendEmail)
            .Then(context =>
                Debug.WriteLine("Email requested to send for saga {0}", context.Saga.CorrelationId)));


        During(SendEmail, When(InviteUpdatedEvent)
            .TransitionTo(Complete)
            .Then(context => Debug.WriteLine("Saga completed for: {0}", context.Saga.CorrelationId)));

        //Will handle failed event raised during any state
        DuringAny(When(InviteFailedEvent)
            .Then(context =>
            {
                //use case will call service to do something
                //can set tenant code on saga here to be used by other mediator requests
                context.Saga.ErrorMessage = context.Message.ErrorMessage;
            })
            .TransitionTo(Failed)
            .Then(context => Debug.WriteLine("Saga Failed for: {0}", context.Saga.CorrelationId)));
        //.Finalize()); //sets saga to final state

        //completes and deletes saga from saga repository when state is final
        //SetCompletedWhenFinalized();
    }

    public Event<InviteUpdatedEvent> InviteUpdatedEvent { get; private set; }
    public Event<InviteFailedEvent> InviteFailedEvent { get; private set; }
}