using System.Diagnostics;
using MassTransit;
using MassTransitPoc.Domain;
using MassTransitPoc.UseCases.CreateBrand;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitPoc.Producers;

public class InviteStateProducerUseCase : IConsumer<InviteStateProducerRequest>
{
    private readonly ITopicProducer<BrandCreatedEvent> _brandCreatedEventProducer;
    private readonly ITopicProducer<UserCreatedEvent> _userCreatedEventProducer;
    private readonly ITopicProducer<InviteCreatedEvent> _inviteCreatedEventProducer;
    private readonly ITopicProducer<EmailSentEvent> _emailSentEventProducer;

    public InviteStateProducerUseCase([FromServices] ITopicProducer<BrandCreatedEvent> brandCreatedEventProducer,
        [FromServices] ITopicProducer<UserCreatedEvent> userCreatedEventProducer,
        [FromServices] ITopicProducer<InviteCreatedEvent> inviteCreatedEventProducer,
        [FromServices] ITopicProducer<EmailSentEvent> emailSentEventProducer)
    {
        _brandCreatedEventProducer = brandCreatedEventProducer;
        _userCreatedEventProducer = userCreatedEventProducer;
        _inviteCreatedEventProducer = inviteCreatedEventProducer;
        _emailSentEventProducer = emailSentEventProducer;
    }

    public async Task Consume(ConsumeContext<InviteStateProducerRequest> context)
    {
        //Call brand service
        Debug.WriteLine($"Producing message of type {context.Message.EventToProduce}");

        //produce event
        switch (context.Message.EventToProduce)
        {
            case nameof(BrandCreatedEvent):

                await _brandCreatedEventProducer.Produce(new BrandCreatedEvent()
                {
                    OperationId = context.Message.OperationId
                });
                break;

            case nameof(UserCreatedEvent):

                await _userCreatedEventProducer.Produce(new BrandCreatedEvent()
                {
                    OperationId = context.Message.OperationId
                });
                break;

            case nameof(InviteCreatedEvent):

                await _inviteCreatedEventProducer.Produce(new BrandCreatedEvent()
                {
                    OperationId = context.Message.OperationId
                });
                break;

            case nameof(EmailSentEvent):

                await _emailSentEventProducer.Produce(new BrandCreatedEvent()
                {
                    OperationId = context.Message.OperationId
                });
                break;

            default:
                Console.WriteLine("Event not supported");
                break;
        }

        await context.RespondAsync(new InviteStateProducerResponse { OperationId = context.Message.OperationId });
    }
}