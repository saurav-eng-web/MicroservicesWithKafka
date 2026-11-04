namespace MicroservicesWithKafka.Models.Logistic
{
    /// <summary>
    /// Row of legacy PrintGoodChallan.aspx grid:
    /// select Gud_No, convert(nvarchar,Gud_Date,101) GudDate, Gud_DocNo, Gud_Remark from Tbl_GudPOMaster
    /// </summary>
    public class GoodPartChallanViewModel
    {
        public string GudNo { get; set; } = string.Empty;
        public string GudDate { get; set; } = string.Empty;
        public string GudDocNo { get; set; } = string.Empty;
        public string GudRemark { get; set; } = string.Empty;
    }
}
