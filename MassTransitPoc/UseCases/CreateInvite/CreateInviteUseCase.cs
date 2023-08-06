using MassTransit;
using MassTransitPoc.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MassTransitPoc.UseCases.CreateInvite;

public class CreateInviteUseCase : IConsumer<CreateInviteRequest>
{
    private readonly ITopicProducer<InviteCreatedEvent> _producer;

    public CreateInviteUseCase([FromServices] ITopicProducer<InviteCreatedEvent> producer)
    {
        _producer = producer;
    }

    public async Task Consume(ConsumeContext<CreateInviteRequest> context)
    {
        Debug.WriteLine("Starting invite saga");

        //produce event to kafka to start saga
        await _producer.Produce(new InviteCreatedEvent
        {
            OperationId = context.Message.OperationId
        });

        await context.RespondAsync(new CreateInviteResponse { OperationId = context.Message.OperationId });
    }
}
