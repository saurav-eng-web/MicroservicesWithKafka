namespace MicroservicesWithKafka.Models
{
    public class ChallanSearchRequestViewModel
    {
        public string? AscName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string? SfId { get; set; }
        public string? ChallanType { get; set; }
    }
}
