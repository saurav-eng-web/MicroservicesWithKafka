namespace MicroservicesWithKafka.Models
{
    public class CallSchedule
    {
        // ID is numeric(18, 0) in SQL -> long in C#
        public long ID { get; set; }

        // SfId is numeric(18, 0) in SQL (changed from int to long)
        public long SfId { get; set; }

        // EngId is numeric(18, 0) in SQL (changed from int? to long?)
        public long? EngId { get; set; }

        public DateTime? AppointmentDate { get; set; }

        // AppointmentTime is nvarchar(100) in SQL -> string? in C#
        public string? AppointmentTime { get; set; }

        public string? TalkToWhomeAllot { get; set; }

        public DateTime? Tdate { get; set; }

        public string? TicketNo { get; set; }

        // TicketKey is numeric(18, 0) in SQL -> long? in C#
        public long? TicketKey { get; set; }

        // Status is BIT in SQL -> bool? in C# (or int? if you prefer 0/1)
        public int? Status { get; set; }

        public DateTime? ApptDateandTime { get; set; }

        // Stored Procedure Output
        public string? StatusMsg { get; set; }
        public int? StatusCode { get; set; }
    }
}
