namespace MicroservicesWithKafka.Models
{
    public class ComplaintAggregationResult
    {
        public int? Bid { get; set; }
        public string ComplaintStatus { get; set; }
        public string ComplaintSubStatus { get; set; }
        public int Count { get; set; }
    }
}
