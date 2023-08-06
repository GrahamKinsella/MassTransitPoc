using MassTransit;
using MassTransitPoc.Domain;

namespace MassTransitPoc.Consumers;

public class InviteFailedEventConsumer : IConsumer<InviteFailedEvent>
{

    public async Task Consume(ConsumeContext<InviteFailedEvent> context)
    {
        await context.RespondAsync(new ConsumerResponse { OperationId = context.Message.OperationId });
    }
}