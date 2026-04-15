namespace MicroservicesWithKafka.Models
{
    public class PORequestModel
    {
        public string TicketNo { get; set; } = string.Empty;
        public object Payload { get; set; }
    }
}
