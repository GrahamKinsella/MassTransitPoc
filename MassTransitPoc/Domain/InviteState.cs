using MassTransit;
using Newtonsoft.Json;

namespace MassTransitPoc.Domain;

public class InviteState : SagaStateMachineInstance
{
    /// <inheritdoc />
    [JsonProperty("id")] 
    public Guid CorrelationId { get; set; }
    public string OperationId { get; set; }

    public string CurrentState { get; set; }
    public string Email { get; set; }
    public string Comments { get; set; }
    public string BrandName { get; set; }
    public string OrganisationId { get; set; }
    public string IsPlanSupported { get; set; }
    public string Plan { get; set; }
    public string Region { get; set; }
    public string PartnerId { get; set; }
    public string Variant { get; set; }
    public string TenantCode { get; set; }
    public string ErrorMessage { get; set; }

    [JsonProperty("_etag")] 
    public string ETag { get; set; }
}