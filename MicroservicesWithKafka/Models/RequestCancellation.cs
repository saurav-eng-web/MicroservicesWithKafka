namespace MicroservicesWithKafka.Models
{
    public class RequestCancellation
    {
        public long? CancellationRequestID { get; set; }
        public string? TransactionType { get; set; }      // e.g. "CallRegister", "Order", "Warranty"
        public string? TransactionID { get; set; }          // ID of the related record
        public string? TransactionNo { get; set; }        // Optional external reference like TicketNo or OrderNo

        // 🔹 Request Information
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string? RequestedBy { get; set; }
        public string? Reason { get; set; }
        public string? Remarks { get; set; }

        // 🔹 Approval Information
        public string ApprovalStatus { get; set; } = "Pending"; // Pending / Approved / Rejected
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovalRemarks { get; set; }

        // 🔹 Audit Information
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // 🔹 System Info
        public bool IsActive { get; set; } = true;
        public string? IPAddress { get; set; }
        public string? StatusMsg { get; set; }
        public int? StatusCode { get; set; }
    }
}
