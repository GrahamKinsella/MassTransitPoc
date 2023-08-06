namespace MassTransitPoc.Producers;

public class InviteStateProducerRequest
{
    public Guid OperationId { get; set; }

    public string EventToProduce { get; set; }
}