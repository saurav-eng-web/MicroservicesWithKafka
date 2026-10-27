namespace MicroservicesWithKafka.Models.Logistic
{
    public class ChallanSearchRequestViewModel
    {
        /// <summary>ASC name — used by the DOA / Good / Good Machine / Mis pages (legacy filtered on asc_name).</summary>
        public string? AscName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        /// <summary>
        /// SF id (ASC_MASTER.ASCINTCODE). Used by PrintChallanList (Defective page) and MisChallanFG
        /// (legacy Session("sidfic")).
        /// </summary>
        public string? SfId { get; set; }
        /// <summary>
        /// Defective page "Challan Type" (legacy PrintDefectiveChallan.aspx dropdown):
        /// Defective | DOA | GoodSpare | GoodFG | MisFG | MisSpare. Defaults to Defective.
        /// </summary>
        public string? ChallanType { get; set; }
    }
}
