using MassTransit;
using MassTransitPoc.Domain;
using System.Diagnostics;

namespace MassTransitPoc.Consumers;

public class SendEmailConsumer : IConsumer<InviteCreatedEvent>
{
    public async Task Consume(ConsumeContext<InviteCreatedEvent> context)
    {

        //Call Email Service
        Debug.WriteLine("Changing state to EmailCreated");

        await context.RespondAsync(new ConsumerResponse { OperationId = context.Message.OperationId });
    }
}