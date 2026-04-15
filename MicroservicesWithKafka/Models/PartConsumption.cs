namespace MicroservicesWithKafka.Models
{
    public class PartConsumption
    {
        public int PC_ID { get; set; }
        public int? PC_JobNo { get; set; }
        public int? PC_Srno { get; set; }
        public string? PC_PartCode { get; set; }
        public string? PC_PartDesc { get; set; }
        public DateTime? PC_Date { get; set; }
        public decimal? PC_Rate { get; set; }
        public int? PC_Quantity { get; set; }
        public decimal? PC_Amount { get; set; }
        public int? PC_SendDef { get; set; }
        public int? PC_DefPOID { get; set; }
        public int? PC_Consumed { get; set; }
        public string? Ticket_no { get; set; }
        public int? Scrapped { get; set; }
        public string? DefPartSerialNo { get; set; }
        public string? PackSlipNo { get; set; }
        public bool? ISReturnable { get; set; }
        public bool? IsReplaced { get; set; }
        public bool? ISShort { get; set; }
        public bool? IsFreshFaulty { get; set; }
        public int? AXStatus { get; set; }
        public string? InventoryType { get; set; }
    }
}
