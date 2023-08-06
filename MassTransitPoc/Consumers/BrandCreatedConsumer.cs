using MassTransit;
using MassTransitPoc.Domain;
using System.Diagnostics;

namespace MassTransitPoc.Consumers;

public class BrandCreatedConsumer : IConsumer<BrandCreatedEvent>
{

    public async Task Consume(ConsumeContext<BrandCreatedEvent> context)
    {
        //call Brand Service
        Debug.WriteLine("Changing state to BrandCreated");

        await context.RespondAsync(new ConsumerResponse { OperationId = context.Message.OperationId });
    }
}