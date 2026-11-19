namespace MicroservicesWithKafka.Models.Logistic
{
    // Translated from StatusView.aspx.vb grdReverseDamageMismatch binding
    public class StatusViewViewModel
    {
        public string ChallanNo      { get; set; } = string.Empty;
        public string TypeOfChallan  { get; set; } = string.Empty;
        public string PickupBoy      { get; set; } = string.Empty;
        public string PickupMobile   { get; set; } = string.Empty;
        public string DocketStatus   { get; set; } = string.Empty;
        /// <summary>Status of the challan itself (DefPO_Status / DefDOANWH_Status / Gud_Status / Mis_Status / PoStatus).</summary>
        public string ChallanStatus  { get; set; } = string.Empty;
    }
}
