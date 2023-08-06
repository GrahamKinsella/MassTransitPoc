using MassTransitPoc.UseCases.Common;

namespace MassTransitPoc.UseCases.CreateBrand;

public class CreateBrandRequest : BaseInviteRequest
{
    public string BrandName { get; set; }
    public string Plan { get; set; }
}