namespace MicroservicesWithKafka.Models.Logistic
{
    public class DashboardDetailViewModel
    {
        public string SourceType    { get; set; } = string.Empty;
        public string ChallanNo     { get; set; } = string.Empty;
        public string NspName       { get; set; } = string.Empty;
        public string AscCode       { get; set; } = string.Empty;
        public string PickupBoy     { get; set; } = string.Empty;
        public string PickupMobile  { get; set; } = string.Empty;
        public string ChallanStatus { get; set; } = string.Empty;
        public string DocketStatus  { get; set; } = string.Empty;
    }
}
