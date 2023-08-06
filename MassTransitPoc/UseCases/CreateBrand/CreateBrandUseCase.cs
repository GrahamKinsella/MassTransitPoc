using MassTransit;
using System.Diagnostics;
using MassTransit.Mediator;
using MassTransitPoc.Producers;

namespace MassTransitPoc.UseCases.CreateBrand;

public class CreateBrandUseCase : IConsumer<CreateBrandRequest>
{
    private readonly IMediator _mediator;

    public CreateBrandUseCase(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task Consume(ConsumeContext<CreateBrandRequest> context)
    {
        //Call brand service
        try
        {
            Debug.WriteLine("Calling Brand Service logic goes here");

            //Produce event - this can be through the platform kafka pkg
            await _mediator.Publish(new InviteStateProducerRequest
            {
                OperationId = context.Message.OperationId,
                Status = "BrandCreated",
                TenantCode = "Get this from the brand service"

            });
        }
        catch (Exception ex)
        {
            //Logger.Error(ex, "Creating Brand Failed for brand {context.Message.BrandName} Invite id {context.Message.OperationId}")
            Debug.WriteLine("Creating Brand Failed");
            await _mediator.Publish(new InviteStateProducerRequest
            {
                Status = "InviteFailed",
                ErrorMessage = "Create Brand Failed"
            });
        }
    }
}
