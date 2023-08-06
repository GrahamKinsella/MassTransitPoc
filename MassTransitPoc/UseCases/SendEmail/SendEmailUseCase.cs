using System.Diagnostics;
using MassTransit;

namespace MassTransitPoc.UseCases.SendEmail;

public class SendEmailUseCase : IConsumer<SendEmailRequest>
{

    public async Task Consume(ConsumeContext<SendEmailRequest> context)
    {
        //Call email service
        Debug.WriteLine("Calling Email Service");

        await context.RespondAsync(new SendEmailResponse() { OperationId = context.Message.OperationId });
    }
}