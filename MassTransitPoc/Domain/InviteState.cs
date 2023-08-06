using MassTransit;
using Newtonsoft.Json;

namespace MassTransitPoc.Domain;

public class InviteState : SagaStateMachineInstance
{
    /// <inheritdoc />
    [JsonProperty("id")] 
    public Guid CorrelationId { get; set; }

    public int CurrentState { get; set; }

    [JsonProperty("_etag")] 
    public string ETag { get; set; }
}