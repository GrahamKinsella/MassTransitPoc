using MassTransit;
using MassTransitPoc.Domain;
using System.Diagnostics;

namespace MassTransitPoc.Consumers;

public class InviteCreatedConsumer : IConsumer<InviteCreatedEvent>
{

    public async Task Consume(ConsumeContext<InviteCreatedEvent> context)
    {
        //call Brand Service
        Debug.WriteLine("Starting Saga");

        await context.RespondAsync(new ConsumerResponse { OperationId = context.Message.OperationId });
    }
}