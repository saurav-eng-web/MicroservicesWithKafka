using System;

namespace MicroservicesWithKafka.Models
{
    public class ASC_MASTER
    {
        public long ASCINTCODE { get; set; }  // Identity PK
        public string? ASC_CODE { get; set; }
        public string? ASC_NAME { get; set; }
        public string? ASC_PATH { get; set; }
        public DateTime? TDATE { get; set; }
        public DateTime? TTIME { get; set; }
        public string? ENTERBY { get; set; }
        public string? ASC_STATUS { get; set; }
        public long? ASC_CatID { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Region { get; set; }
        public string? Branch { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? MobileNo { get; set; }
        public string? SAC { get; set; }
        public string? SHA { get; set; }
        public string? SCE { get; set; }
        public string? SSD { get; set; }
        public string? FTP { get; set; }
        public string? Asc_Category { get; set; }
        public string? ASC_Category1 { get; set; }
        public string? ASC_Category2 { get; set; }
        public string? CatID1 { get; set; }
        public string? CatID2 { get; set; }
        public string? CatID3 { get; set; }
        public string? Pan_Number { get; set; }
        public string? ST_Number { get; set; }
        public DateTime? DOJ { get; set; }
        public string? SAPCode { get; set; }
        public long? RegionId { get; set; }
        public long? Bid { get; set; }
        public string? CotPerson { get; set; }
        public long? CityId { get; set; }
        public long? Locality { get; set; }
        public bool? SendSms { get; set; }
        public decimal? MinGaurantee { get; set; }
        public decimal? SecurityAmount { get; set; }
        public long? AvgCallLoad { get; set; }
        public string? BankName { get; set; }
        public string? AccountNo { get; set; }
        public bool? PORecieveByAdmin { get; set; }
        public string? TinNo { get; set; }
        public string? NAVCode { get; set; }
        public long? Pincode { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public long? StateID { get; set; }
        public string? AccountHolderName { get; set; }
        public string? BankBranch { get; set; }
        public string? IFSCCode { get; set; }
        public long? ASM_ID { get; set; }
        public string? ASC_GST { get; set; }
        public string? ASC_State { get; set; }
        public string? ASC_StateCode { get; set; }
        public string? BranchWarehouse { get; set; }
        public string? CorrectionWarehouse { get; set; }
        public string? CorrectionWarehousepincode { get; set; }
    }
}
