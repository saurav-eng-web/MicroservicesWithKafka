using System;

namespace MicroservicesWithKafka.Models
{
    public class Visit
    {
        public long? SlNo { get; set; }
        public long? VisitId { get; set; }
        public long? JobNo { get; set; }
        public string? VisitDetail { get; set; }
        public string? EngName { get; set; }
        public DateTime? VDate { get; set; }
        public string? VInTime { get; set; }
        public string? VOutTime { get; set; }
        public string? VStatus { get; set; }
        public string? Remark { get; set; }
        public DateTime? TDate { get; set; }
        public DateTime? TTime { get; set; }
        public DateTime? DownloadDateTime { get; set; }
        public string? AsfCode { get; set; }
        public long? EngId { get; set; }
        public bool? Tr { get; set; }
        public string? CompStatus { get; set; }
        public string? TicketNo { get; set; }
    }

}
