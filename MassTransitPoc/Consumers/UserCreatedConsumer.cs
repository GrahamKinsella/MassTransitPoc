using MassTransit;
using MassTransitPoc.Domain;
using System.Diagnostics;

namespace MassTransitPoc.Consumers;

public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
{
    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {

        //Call User Service
        Debug.WriteLine("Changing state to UserCreated");

        await context.RespondAsync(new ConsumerResponse { OperationId = context.Message.OperationId });
    }
}