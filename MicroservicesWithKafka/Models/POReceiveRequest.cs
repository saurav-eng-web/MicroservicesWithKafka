namespace MicroservicesWithKafka.Models
{
    public class POReceiveRequest
    {
        public string Type { get; set; }     // Pending / PendingSearch
        public int Sf_Id { get; set; }
        public string? Search { get; set; }

        public object? Payload { get; set; }               // GENERIC JSON
    }
}
