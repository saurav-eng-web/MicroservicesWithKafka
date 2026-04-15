using System;

namespace MicroservicesWithKafka.Models
{
    public class RssoftechSMS
    {
        public decimal ID { get; set; }
        public string Source { get; set; }
        public string Mobile { get; set; }
        public string Message { get; set; }
        public DateTime? Tdate { get; set; }
        public DateTime? SendDateTime { get; set; }
        public bool Sent { get; set; }
        public string Status { get; set; }
        public string REF { get; set; }
        public string TicketKey { get; set; }
        public decimal? APIID { get; set; }
    }
}
