using System;

namespace MicroservicesWithKafka.Models
{
    public class ClaimProcessModel
    {
        public int UserId { get; set; }
        public string? Action { get; set; }
        public string? Type { get; set; }
        public string? RecId { get; set; }
        public string? Remarks { get; set; }
        public string? StatusReason { get; set; }

        public string? DopInput { get; set; }
        public string? ModelNoInput { get; set; }
        public string? SerialNoInput { get; set; }

        public string? Nsp { get; set; }
        public string? Ct { get; set; }
        public string? Sst { get; set; }
        public string? Tk1 { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
