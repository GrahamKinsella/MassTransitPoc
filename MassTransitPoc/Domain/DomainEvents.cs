namespace MassTransitPoc.Domain;

//Events
public record InviteCreatedEvent
{
    public Guid OperationId { get; set; }
}

public record BrandCreatedEvent
{
    //Operationid from 
    public Guid OperationId { get; set; }
}

public record UserCreatedEvent
{
    public Guid OperationId { get; set; }
}

public record EmailSentEvent
{
    public Guid OperationId { get; set; }
}