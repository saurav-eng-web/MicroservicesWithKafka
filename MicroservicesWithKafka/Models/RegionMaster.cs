namespace MicroservicesWithKafka.Models
{
    public class RegionMaster
    {
        public decimal? Rid { get; set; }
        public string? Regionname { get; set; }
        public DateTime? Tdate { get; set; }
        public DateTime? Ttime { get; set; }
        public string? Status { get; set; }
        public string? SN { get; set; }
    }
}
