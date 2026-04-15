namespace MicroservicesWithKafka.Models
{
    public class Reminder
    {
        public long? Remind_ID { get; set; }           // Identity PK
        public long? Remind_Bid { get; set; }
        public long? Remind_Sfid { get; set; }
        public long? Remind_Custintcode { get; set; }
        public long? Remind_TicketNo { get; set; }
        public string? Remind_Remark { get; set; }
        public DateTime Remind_date { get; set; }     // Default = GETDATE()
        public string? Remind_Status { get; set; }
        public string? Remind_Enterby { get; set; }
        public string? Remind_Trap { get; set; }

        public string? Ticket_No { get; set; }
    }
}
