namespace MicroservicesWithKafka.Models.Logistic
{
    public class DocketSearchRequestViewModel
    {
        /// <summary>Challan type e.g. DefectivePart</summary>
        public string? Type { get; set; }
        public string? ChallanNo { get; set; }
    }
}
