using MassTransit;
using MassTransitPoc.Domain;
using System.Diagnostics;
using MassTransit.Mediator;
using MassTransitPoc.UseCases.CreateBrand;

namespace MassTransitPoc.Consumers;

public class InviteUpdatedEventConsumer : IConsumer<InviteUpdatedEvent>
{
    private readonly IMediator _mediator;

    public InviteUpdatedEventConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<InviteUpdatedEvent> context)
    {

        switch (context.Message.Status)
        {
            case "CreateBrand":
                Debug.WriteLine("invoking use case to call brand service to create brand");
                var client = _mediator.CreateRequestClient<CreateBrandRequest>();
                var response = await client.GetResponse<CreateBrandResponse>(new CreateBrandRequest
                    { BrandName = context.Message.BrandName, Plan = context.Message.Plan });
                break;

            case "CreateUser":
                Debug.WriteLine("invoking use case to call user service to create user");
                break;

            case "SendEmail":
                Debug.WriteLine("invoking use case to call email service to send email");
                break;
        }

        await context.RespondAsync(new ConsumerResponse { OperationId = context.Message.OperationId, TenantCode = "Test"});
    }
}