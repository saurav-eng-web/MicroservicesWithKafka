namespace MicroservicesWithKafka.Models
{
    public class PODetailRequest
    {
        public string Action { get; set; }

        // Identity
        public long? Id { get; set; }

        public long? POMasterID { get; set; }
        public string? Ticket_Key { get; set; }
        public long? ModelID { get; set; }
        public string? Partcode { get; set; }
        public string? PartDesc { get; set; }
        public decimal? Price { get; set; }
        public long? qty { get; set; }

        // BIT → int (0/1)
        public int? ProcessToBranch { get; set; }

        public string? ERPNO { get; set; }
        public string? DOCNo { get; set; }

        public DateTime? DispatchDate { get; set; }
        public DateTime? DispatchTime { get; set; }

        public string? Remark { get; set; }

        // BIT → int (0/1)
        public int? Dispatch { get; set; }

        public int? DispatchQty { get; set; }
        public long? RequiredQty { get; set; }
        public long? BalanceQty { get; set; }

        // BIT → int (0/1)
        public int? Backorder { get; set; }

        // In DB BackorderPO is NVARCHAR(500) → keep as string
        public string? BackorderPO { get; set; }

        public string? Status { get; set; }

        // BIT → int (0/1)
        public int? Recieve { get; set; }

        public string? StatusRemark { get; set; }

        public long? BranchToNWH { get; set; }

        public DateTime? SendNWHDate { get; set; }

        // BIT → int (0/1)
        public int? SendNWH { get; set; }
        public int? SendVendor { get; set; }

        public string? despby { get; set; }
        public long? despid { get; set; }

        public string? Courier { get; set; }
        public decimal? CourierAmt { get; set; }

        public DateTime? RecieveDate { get; set; }

        public string? PackSlipNo { get; set; }

        public long? SFID { get; set; }

        // BIT → int (0/1)
        public int? IsPackingSlipUsed { get; set; }

        public string? NwhSerialNumber { get; set; }

        public string? InventoryType { get; set; }

        // BIT → int (0/1)
        public int? ValueApproval { get; set; }

        public string? ImageUpload { get; set; }

        public string? PoCancelReason { get; set; }

        public DateTime? PoCancelDate { get; set; }

        public DateTime? CreatedDate { get; set; }
    }

}
