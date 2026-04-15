namespace MicroservicesWithKafka.Models
{
    public class NwhDefectiveRequest
    {
        public string Action { get; set; }
        public long? DefNwh_ID { get; set; }
        public string? DefNwh_Partcode { get; set; }
        public string? DefNwh_Description { get; set; }
        public int? DefNwh_Qty { get; set; }
        public int? nwhcatid { get; set; }
    }
}
