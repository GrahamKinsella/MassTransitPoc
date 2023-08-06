using MassTransitPoc.UseCases.Common;

namespace MassTransitPoc.UseCases.SendEmail;

public class SendEmailRequest : BaseInviteRequest
{
    public string Email { get; set; }
    public string Comments { get; set; }
}