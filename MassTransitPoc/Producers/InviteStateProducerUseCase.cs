using System.Diagnostics;
using MassTransit;
using MassTransitPoc.Domain;
using MassTransitPoc.UseCases.CreateBrand;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitPoc.Producers;

public class InviteStateProducerUseCase : IConsumer<InviteStateProducerRequest>
{
    private readonly ITopicProducer<InviteUpdatedEvent> _inviteUpdatedEventProducer;
    private readonly ITopicProducer<InviteFailedEvent> _failedEventProducer;


    public InviteStateProducerUseCase([FromServices] ITopicProducer<InviteUpdatedEvent> inviteUpdatedEventProducer,
        [FromServices] ITopicProducer<InviteFailedEvent> failedEventProducer)
    {
        _inviteUpdatedEventProducer = inviteUpdatedEventProducer;
        _failedEventProducer = failedEventProducer;
    }

    public async Task Consume(ConsumeContext<InviteStateProducerRequest> context)
    {
        //Call brand service
        Debug.WriteLine($"Producing message with status {context.Message.Status}");

        //produce event
        switch (context.Message.Status)
        {
            case "Failed":

                await _failedEventProducer.Produce(new InviteFailedEvent()
                {
                    OperationId = context.Message.OperationId
                });
                break;

            default:
                await _inviteUpdatedEventProducer.Produce(new InviteUpdatedEvent
                {
                    OperationId = context.Message.OperationId,
                    Status = context.Message.Status,
                    Email = context.Message.Email,
                    BrandName = context.Message.BrandName,
                    Comments = context.Message.Comments,
                    IsPlanSupported = context.Message.IsPlanSupported,
                    OrganisationId = context.Message.OrganisationId,
                    PartnerId = context.Message.PartnerId,
                    Plan = context.Message.Plan,
                    Region = context.Message.Region


                });
                break;

        }

        await context.RespondAsync(new InviteStateProducerResponse { OperationId = context.Message.OperationId });
    }
}