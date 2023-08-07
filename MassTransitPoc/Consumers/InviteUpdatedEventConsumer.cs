using MassTransit;
using MassTransitPoc.Domain;
using System.Diagnostics;
using MassTransit.Mediator;
using MassTransitPoc.UseCases.CreateBrand;
using MassTransitPoc.UseCases.CreateUser;
using MassTransitPoc.UseCases.SendEmail;

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
            case "InviteCreated":
                Debug.WriteLine("invoking use case to call brand service to create brand");

                await _mediator.Send(new CreateBrandRequest
                {
                    OperationId = context.Message.OperationId, BrandName = context.Message.BrandName,
                    Plan = context.Message.Plan
                }); //theres more here not adding them 
                break;

            case "BrandCreated":
                await _mediator.Send(new CreateUserRequest
                {
                    OperationId = context.Message.OperationId, Email = context.Message.Email,
                    TenantCode = context.Message.TenantCode
                });
                Debug.WriteLine("invoking use case to call user service to create user");
                break;

            case "UserCreated":
                await _mediator.Send(new SendEmailRequest
                {
                    OperationId = context.Message.OperationId, Email = context.Message.Email,
                    Comments = context.Message.Comments
                });
                Debug.WriteLine("invoking use case to call email service to send email");
                break;

            case "Complete":
                Debug.WriteLine("Invite Saga is complete");
                break;

        }
    }
}