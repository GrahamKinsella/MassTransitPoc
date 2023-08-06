using System.Diagnostics;
using MassTransit;
using MassTransit.Mediator;
using MassTransitPoc.Producers;

namespace MassTransitPoc.UseCases.SendEmail;

public class SendEmailUseCase : IConsumer<SendEmailRequest>
{
    private readonly IMediator _mediator;

    public SendEmailUseCase(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<SendEmailRequest> context)
    {
        //Call brand service
        try
        {
            Debug.WriteLine("Calling Email Service logic goes here");

            //Produce event - this can be through the platform kafka pkg
            await _mediator.Publish(new InviteStateProducerRequest
            {
                OperationId = context.Message.OperationId,
                Status = "Completed",
            });
        }
        catch (Exception ex)
        {
            //Logger.Error(ex, "Sending Email Failed for brand {context.Message.BrandName} Invite id {context.Message.OperationId}")
            Debug.WriteLine("Sending Email Failed");
            await _mediator.Publish(new InviteStateProducerRequest
            {
                Status = "InviteFailed",
                ErrorMessage = "Sending Email Failed"
            });
        }
    }
}