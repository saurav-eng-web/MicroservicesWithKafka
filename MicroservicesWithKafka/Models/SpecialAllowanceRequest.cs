namespace MicroservicesWithKafka.Models
{
    public class SpecialAllowanceRequest
    {
        public string Action { get; set; }
        public string? Ticket_No { get; set; }

        public string? SpecialApprovaltype { get; set; }
        public decimal? NSPRequestAmount { get; set; }
        public string? NSPRemarks { get; set; }
        public string? NSPRequestBy { get; set; }

        public decimal? RSMApproveAmount { get; set; }
        public string? RSMRemarks { get; set; }
        public string? RSMApproveBy { get; set; }

        public decimal? HOApproveAmount { get; set; }
        public string? HORemarks { get; set; }
        public string? HOApprovedBy { get; set; }

        public string? sa_status { get; set; }

        public int? BID { get; set; }
        public int? SFID { get; set; }
        public int? RSMID { get; set; }
        public int? ASMID { get; set; }
    }
}
