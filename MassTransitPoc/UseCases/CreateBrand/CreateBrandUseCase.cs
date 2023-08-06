using MassTransit;
using System.Diagnostics;

namespace MassTransitPoc.UseCases.CreateBrand;

public class CreateBrandUseCase : IConsumer<CreateBrandRequest>
{
    public async Task Consume(ConsumeContext<CreateBrandRequest> context)
    {

        //Call brand service
        Debug.WriteLine("Calling Brand Service");

        await context.RespondAsync(new CreateBrandResponse { TenantCode = "Test"});
    }
}
