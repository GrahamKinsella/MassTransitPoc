using System.Diagnostics;
using MassTransit;

namespace MassTransitPoc.UseCases.CreateUser;

public class CreateUserUseCase : IConsumer<CreateUserRequest>
{

    public async Task Consume(ConsumeContext<CreateUserRequest> context)
    {

        //Call user service
        Debug.WriteLine("Calling User Service");

        await context.RespondAsync(new CreateUserResponse() { OperationId = context.Message.OperationId });

    }
}
