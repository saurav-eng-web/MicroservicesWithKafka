namespace MicroservicesWithKafka.Models.Logistic
{
    public class DocketViewModel
    {
        public int? Id { get; set; }
        public string? ChallanNo { get; set; }
        public string? CourierCode { get; set; }
        public string? DocketNo { get; set; }
        public string? Type { get; set; }
        public string? PickupDate { get; set; }
        public string? DeliveryDate { get; set; }
        public string? DocStatus { get; set; }
    }
}
