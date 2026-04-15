using System;

namespace MicroservicesWithKafka.Models
{
    public class PartMaster
    {
        public decimal? PartID { get; set; }
        public decimal? PartSerialNo { get; set; }
        public decimal? ProductID { get; set; }
        public decimal? ModelID { get; set; }
        public decimal? Prd_Color { get; set; }
        public string PartCode { get; set; }
        public string PartDescription { get; set; }
        public decimal? CustomerPrice { get; set; }
        public decimal? SPR_FactoryID { get; set; }
        public decimal? SPR_Tax { get; set; }
        public decimal? SprColor { get; set; }
        public decimal? Spr_Cost { get; set; }
        public string Model { get; set; }
        public string Status { get; set; }
        public string Isuploaded { get; set; }
        public DateTime? UploadedDateTime { get; set; }
        public decimal? CategoryID { get; set; }
        public string Category { get; set; }
        public decimal? SFPrice { get; set; }
        public decimal? AVCost { get; set; }
        public decimal? PPrice { get; set; }
        public decimal? MSLQty { get; set; }
        public string Location { get; set; }
        public bool? Returnable { get; set; }
        public bool? Tosell { get; set; }
        public decimal? MinLeadTime { get; set; }
        public decimal? MaxLeadTime { get; set; }
        public decimal? AvgLeadTime { get; set; }
        public decimal? MOQ { get; set; }
        public bool? Repairable { get; set; }
        public string TallyCode { get; set; }
        public decimal? BOMQty { get; set; }
        public bool? IsSerialized { get; set; }
        public bool? IsWallMount { get; set; }
        public string HSN { get; set; }
        public decimal? Weight { get; set; }
        public string BomType { get; set; }
        public int? ExWarrantyDays { get; set; }
        public bool? ValueApproval { get; set; }
        public string SafteyBox { get; set; }
        public string Paidwarranty { get; set; }
    }
}
