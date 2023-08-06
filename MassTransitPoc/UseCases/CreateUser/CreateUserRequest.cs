using MassTransitPoc.UseCases.Common;

namespace MassTransitPoc.UseCases.CreateUser;

public class CreateUserRequest : BaseInviteRequest
{
    public string Email { get; set; }
    public string TenantCode { get; set; }
}