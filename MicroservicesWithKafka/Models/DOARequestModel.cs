namespace MicroservicesWithKafka.Models
{
    public class DOARequestModel
    {
        public string Type { get; set; } = string.Empty;
        public string? TicketNo { get; set; }
        public object? Payload { get; set; }   // ANY JSON (future-proof)
    }
}
