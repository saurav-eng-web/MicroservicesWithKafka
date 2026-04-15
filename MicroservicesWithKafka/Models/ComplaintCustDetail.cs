using System;

namespace MicroservicesWithKafka.Models
{
    public class ComplaintCustDetail
    {
        public int CUSTINTCODE { get; set; }
        public string? TICKET_NO { get; set; }
        public int TICKET_KEY { get; set; }
        public string? Category { get; set; }
        public string? PRODUCT { get; set; }
        public int Product_ID { get; set; }
        public string? MODEL { get; set; }
        //Asif
        public string? ASC_Pincode { get; set; }

        public string? Distance { get; set; }

        //End ASIF
        // Note: Dapper will map the 'description' column from the SP 
        // to this 'ModelDesc' property if you use an Alias in the SP, 
        // e.g., 'description AS ModelDesc'. If not, this property must be 
        // named 'description' to match the column name. Assuming your SP 
        // returns 'description' based on your old MapToModel.
        public string? ModelDesc { get; set; }
        public int Model_ID { get; set; }
        public string? SERIAL_NO { get; set; }
        public string? ASC_NAME { get; set; }
        public string? ASC_CODE { get; set; }
        public int? Sf_ID { get; set; }
        public DateTime? PURCHASE_DATE { get; set; }
        public DateTime? PROMISE_DATE { get; set; }
        public string? PROMISE_TIME { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public string? LAST_MODIFIED_TIME { get; set; }
        public int? CANCELLED { get; set; }
        public string? CALL_TYPE { get; set; }
        public string? CALL_BY { get; set; }
        public string? REMARKS { get; set; }
        public string? WARRANTY { get; set; }
        public string? COMPLAINT_STATUS { get; set; }
        public DateTime? TDATE { get; set; }
        // TTIME field was mapped as DateTime? in your manual code
        public DateTime? TTIME { get; set; }
        public string? ENTERBY { get; set; }
        public string? DLCity { get; set; }
        public int? City_ID { get; set; }
        public string? Dealer { get; set; }
        public string? DealerMobno { get; set; }
        public int? Dealer_ID { get; set; }
        public string? Call_Category { get; set; }
        public int? CancelCall { get; set; }
        public string? REASON { get; set; }
        public DateTime? ALLOCATIONDATE { get; set; }
        public string? ALLOCATIONTIME { get; set; }
        public int? Eng_ID { get; set; }
        public string? EngName { get; set; }
        public string? CUSTSAT { get; set; }
        public string? CUSTREMARK { get; set; }
        public int? HW_ID { get; set; }
        public string? Contract { get; set; }
        public string? Complaint { get; set; }
        public string? PSSclose { get; set; }
        public DateTime? Amc_Startdate { get; set; }
        public DateTime? Amc_Enddate { get; set; }
        public int? SmsCreated { get; set; }
        public int? Bid { get; set; }
        public int? Call_TypeID { get; set; }
        public int? CallBy_ID { get; set; }
        public int? WarrantyID { get; set; }
        public string? Repair_Class { get; set; }
        public string? Repair_Type { get; set; }
        public string? CityUpcountry { get; set; }
        public decimal? UpcountryDistance { get; set; }
        public decimal? ActualDistance { get; set; }
        public string? InvoiceNo { get; set; }
        public string? BranchName { get; set; }
        public int? Rallot { get; set; }
        public string? OKPSS { get; set; }
        public string? TalkToWhome { get; set; }
        public string? RepairTypeText { get; set; }
        public string? ReopenRemark { get; set; }
        public DateTime? ContactDate { get; set; }
        public string? ContactTime { get; set; }
        public DateTime? ApptDate { get; set; }
        public string? ApptTime { get; set; }
        public string? TalkToWhomeAllot { get; set; }
        public string? ComplaintTrap { get; set; }
        public string? Reopentrap { get; set; }
        public string? CloseTrap { get; set; }
        public string? CloseRemark { get; set; }
        public string? PSSCancelRemark { get; set; }
        public string? FackCallRemark { get; set; }
        public string? ACDealerName { get; set; }
        public string? CustomerType { get; set; }
        public string? ELSSlipno { get; set; }
        public string? ELSStatus { get; set; }
        public string? PrdPhysicalCondition { get; set; }
        public string? CustApp { get; set; }
        public string? IMIE1 { get; set; }
        public string? IMIE2 { get; set; }
        public string? Accessory { get; set; }
        public DateTime? LCDInstOn { get; set; }
        public string? StockOf { get; set; }
        public string? SplInt { get; set; }
        public string? CallSource { get; set; }
        public string? PurchaseFrom { get; set; }
        public string? Email { get; set; }
        public DateTime? CallAcceptRejectTime { get; set; }
        public string? Call_Accept_Reject { get; set; }
        public string? Accept_Reject { get; set; }
        public string? CallRejectReason { get; set; }
        public string? PurchaseSubSource { get; set; }
        public string? CallReason { get; set; }
        public string? CallNature { get; set; }
        public DateTime? ReopenDate { get; set; }
        public string? PendingAtLevel { get; set; }
        public string? Complaint_SubStatus { get; set; }
        public string? PurchaseSubSourceOthers { get; set; }
        public int? RegionID { get; set; }
        public string? CallTransferBY { get; set; }
        public string? CallTransferRemark { get; set; }
        public int? IsApprovedByRSM { get; set; }
        public string? CallClosedReason { get; set; }
        public string? PendingReason { get; set; }
        public string? SubType { get; set; }
        public string? JobDone { get; set; }
        public string? ClosedFrom { get; set; }
        public string? TransferRemark { get; set; }
        public string? TransferReason { get; set; }
        public int? CNPSApproved { get; set; }
        public int? Excalated { get; set; }
        public int? Legal { get; set; }
        public int? ASMID { get; set; }
        public string? ASMName { get; set; }
        public string? Wallmount_Available { get; set; }
        public int? RSMID { get; set; }
        public int? AppointmentCount { get; set; }
        public int? OW_Chk { get; set; }
        public string? OW_Reason { get; set; }
        public string? TD_Doa_Type { get; set; }
        public DateTime? OKDate { get; set; }
        public DateTime? DefectiveSendBySFDate { get; set; }
        public DateTime? DefectiveRecievedByNWHDate { get; set; }
        public int? IsPay { get; set; }
        public string? NotPayWhy { get; set; }
        public DateTime? FinalCloseDate { get; set; }
        public string? DOA_TD_ApprovedType { get; set; }
        public string? CancellationPreStatus { get; set; }
        public string? InvalidSerialNo { get; set; }
        public int? PaymentNotPay { get; set; }
        public string? PaymentNotPayRemark { get; set; }
        public int? TatReasonID { get; set; }
        public string? TatReason { get; set; }
        public string? CancellationRemark { get; set; }
        public string? BomType { get; set; }
        public string? HighValueStatus { get; set; }
        // Customer Fields
        public string? Mobile { get; set; }
        public string? CustName { get; set; }
        public string? HappyCode { get; set; }
        public string? HappyCodeV { get; set; }
        public string? IPAddress { get; set; }
        public DateTime? ReAllotDateTime { get; set; }
        public int? Cust_ID { get; set; }
        public int? AMC_SalesID { get; set; }
        public string? SALUTATION { get; set; }
        public string? FIRSTNAME { get; set; }
        public string? MIDDLENAME { get; set; }
        public string? LASTNAME { get; set; }
        public string? ADDRESS1 { get; set; }
        public string? ADDRESS2 { get; set; }
        public string? ADDRESS3 { get; set; }
        public string? StdCode { get; set; }
        public string? CITY { get; set; }
        public string? PINCODE { get; set; }
        public string? HMPH { get; set; }
        public string? WKPH { get; set; }
        public DateTime? DOB { get; set; }
        public string? AREA_CD_HOME { get; set; }
        public string? AREA_CD_OFFICE { get; set; }
        public string? STATE { get; set; }
        public string? Landmark { get; set; }
        public string? AddedByDlr { get; set; }
        public string? RCOMP { get; set; }
        public DateTime? UploadedDateTime { get; set; }
        public string? Locality { get; set; }
        public int? Locality_ID { get; set; }
        public int? TransferdCRM { get; set; }
        public string? FromWeb { get; set; }
        // Tbl_MstSfEngg Table Field
        public string? Eng_Name { get; set; }
    }
}