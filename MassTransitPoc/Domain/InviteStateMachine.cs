using System.Diagnostics;
using MassTransit;
using MassTransit.Mediator;
using MassTransitPoc.Producers;
using MassTransitPoc.UseCases.CreateBrand;
using MassTransitPoc.UseCases.CreateInvite;
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
    public State Failed { get; private set; }


    public InviteStateMachine(IMediator mediator)
    {
        // Tell the saga where to store the current state
        // when ints here theyre assigned values. In this case
        // 0 - None, 1 - Initial, 2 - Final, 3 - BrandCreated, 4 - UserCreated, 5 - EmailSent
        InstanceState(x => x.CurrentState);

        //On events that are in the Initial state, a new instance of the saga will be created. You can use the SetSagaFactory to control how the saga is instantiated.
        Event(() => InviteCreatedEvent,
            e => e.CorrelateById(cxt => cxt.Message.OperationId));
        Event(() => BrandCreatedEvent,
            e => e.CorrelateById(cxt => cxt.Message.OperationId));
        Event(() => UserCreatedEvent,
            e => e.CorrelateById(cxt => cxt.Message.OperationId));
        Event(() => EmailSentEvent,
            e => e.CorrelateById(cxt => cxt.Message.OperationId));
        Event(() => FailedEvent,
            e => e.CorrelateById(cxt => cxt.Message.OperationId));

        //Can't use a single event and multiple status. State machine has to react to events to change
        Initially(
            When(InviteCreatedEvent) // the event that should trigger this process flow
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
                    //use case will call service to do something

                    var client = mediator.CreateRequestClient<CreateBrandRequest>();
                    var response = await client.GetResponse<CreateBrandResponse>(new CreateBrandRequest
                    { BrandName = context.Saga.BrandName, Plan = context.Saga.Plan });

                    if (!string.IsNullOrEmpty(response.Message.ErrorMessage))
                    {
                        //Produce a Failed event. "Brand Failed to Create"
                    }
                    else
                    {
                        //can set tenant code on saga here to be used by other mediator requests
                        context.Saga.TenantCode = response.Message.TenantCode;
                    }
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

        During(BrandCreated, When(BrandCreatedEvent)
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
            .Then(async context =>
            {
                await mediator.Publish<SendEmailRequest>(new { OperationId = context.Message.OperationId });
            })
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
            .Then(context => Debug.WriteLine("Saga completed for: {0}", context.Saga.CorrelationId)));

        //Will handle failed event raised during any state
        DuringAny(When(FailedEvent)
            .Then( context =>
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

    public Event<InviteCreatedEvent> InviteCreatedEvent { get; private set; }
    public Event<BrandCreatedEvent> BrandCreatedEvent { get; private set; }
    public Event<UserCreatedEvent> UserCreatedEvent { get; private set; }
    public Event<EmailSentEvent> EmailSentEvent { get; private set; }
    public Event<FailedEvent> FailedEvent { get; private set; }
}