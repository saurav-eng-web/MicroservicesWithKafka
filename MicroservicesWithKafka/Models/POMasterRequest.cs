namespace MicroservicesWithKafka.Models
{
    public class POMasterRequest
    {
        public string? Action { get; set; }
        public long? Id { get; set; }
        public string? PONumber { get; set; }
        public string? Ticket_No { get; set; }
        public long? Ticket_Key { get; set; }
        public long? SFID { get; set; }
        public string? Status { get; set; }
        public string? StatusRemark { get; set; }
        public long? BranchToNWH { get; set; }
        public DateTime? SendNWHDate { get; set; }

        // changed bool → int
        public int? SendNWH { get; set; }
        public int? SendVendor { get; set; }
        public int? TOInvSF { get; set; }

        public string? InvoiceNo { get; set; }
        public string? Courier { get; set; }
        public long? TOInvSFid { get; set; }
        public decimal? CourierAmt { get; set; }
        public int? Nwhcatid { get; set; }
        public string? InventoryType { get; set; }
        public string? PoType { get; set; }
        public DateTime? PoCancelDate { get; set; }
        public string? PoCancelReason { get; set; }
    }
}
