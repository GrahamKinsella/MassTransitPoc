namespace MassTransitPoc.Consumers
{
    public class ConsumerResponse
    {
        public Guid OperationId { get; set; }
        public string TenantCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
