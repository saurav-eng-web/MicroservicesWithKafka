namespace MicroservicesWithKafka.Models
{
    public class LevelPendingHistory
    {
        public int? ID { get; set; }
        public string Ticket_No { get; set; }
        public string Ticket_Key { get; set; }
        public string PendingAt { get; set; }
    }
}
