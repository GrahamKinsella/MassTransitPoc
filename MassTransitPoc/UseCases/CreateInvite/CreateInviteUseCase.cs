using MassTransit;
using MassTransit.Mediator;
using MassTransitPoc.Producers;
using System.Diagnostics;

namespace MassTransitPoc.UseCases.CreateInvite;

public class CreateInviteUseCase : IConsumer<CreateInviteRequest>
{
    private readonly IMediator _mediator;

    public CreateInviteUseCase(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<CreateInviteRequest> context)
    {
        try
        {
            Debug.WriteLine("Starting invite saga");

            //Produce event - this can be through the platform kafka pkg
            await _mediator.Publish(new InviteStateProducerRequest
            {
                Status = "InviteCreated",
                OperationId = context.Message.OperationId
            });
        }
        catch (Exception ex)
        {
            //Logger.Error(ex, "Creating Invite Failed for brand {context.Message.BrandName} Invite id {context.Message.OperationId}")
            Debug.WriteLine("Creating Invite Failed");
            await _mediator.Publish(new InviteStateProducerRequest
            {
                OperationId = context.Message.OperationId,
                Status = "InviteFailed",
                ErrorMessage = "Create Brand Failed"
            });
        }
    }
}