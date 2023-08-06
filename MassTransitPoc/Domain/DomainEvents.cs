namespace MassTransitPoc.Domain;

//Events
public record InviteUpdatedEvent
{
    public Guid OperationId { get; set; }
    public string Email { get; set; }
    public string Comments { get; set; }
    public string BrandName { get; set; }
    public string OrganisationId { get; set; }
    public string IsPlanSupported { get; set; }
    public string Plan { get; set; }
    public string Region { get; set; }
    public string PartnerId { get; set; }
    public string Variant { get; set; }
    public string Status { get; set; }
}

public record BrandCreatedEvent
{
    //Operationid from 
    public Guid OperationId { get; set; }
    public string TenantCode {get; set; }
}

public record UserCreatedEvent
{
    public Guid OperationId { get; set; }
}

public record EmailSentEvent
{
    public Guid OperationId { get; set; }
}

public record InviteFailedEvent
{
    public Guid OperationId { get; set; }
    public string ErrorMessage { get; set; }
}