using System.Diagnostics;
using MassTransit;
using MassTransit.Mediator;
using MassTransitPoc.Producers;
using MassTransitPoc.UseCases.CreateBrand;

namespace MassTransitPoc.UseCases.CreateUser;

public class CreateUserUseCase : IConsumer<CreateUserRequest>
{
    private readonly IMediator _mediator;

    public CreateUserUseCase(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<CreateUserRequest> context)
    {
        //Call brand service
        try
        {
            Debug.WriteLine("Calling Brand Service logic goes here");

            //Produce event - this can be through the platform kafka pkg
            await _mediator.Publish(new InviteStateProducerRequest
            {
                OperationId = context.Message.OperationId,
                Status = "UserCreated",
            });
        }
        catch (Exception ex)
        {
            //Logger.Error(ex, "Creating User Failed for brand {context.Message.BrandName} Invite id {context.Message.OperationId}")
            Debug.WriteLine("Creating User Failed");
            await _mediator.Publish(new InviteStateProducerRequest
            {
                Status = "InviteFailed",
                ErrorMessage = "Create User Failed"
            });
        }
    }
}