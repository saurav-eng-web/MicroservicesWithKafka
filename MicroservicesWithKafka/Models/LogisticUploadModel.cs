using System;

namespace MicroservicesWithKafka.Models
{
    public class LogisticUploadModel
    {
        public string TicketNumber { get; set; }
        public string SerialNumber { get; set; }
        public string CourierName { get; set; }
        public string DocketNo { get; set; }
        public string docStatus { get; set; }
        public DateTime? StatusDate { get; set; }

        // Set server-side from the JWT, never from the sheet.
        // IsProcess / TDate are left to the table defaults.
        public string UploadedBy { get; set; }
    }
}
