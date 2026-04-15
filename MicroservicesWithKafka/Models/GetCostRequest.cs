namespace MicroservicesWithKafka.Models
{
    public class GetCostRequest
    {
        public int? SfId { get; set; }
        public decimal RecentPrice { get; set; }
    }
}
