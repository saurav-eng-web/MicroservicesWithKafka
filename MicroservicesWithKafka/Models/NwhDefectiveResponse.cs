namespace MicroservicesWithKafka.Models
{
    public class NwhDefectiveResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public long? DefNwh_ID { get; set; }
        public string? DefNwh_Partcode { get; set; }
        public string? DefNwh_Description { get; set; }
        public int? DefNwh_Qty { get; set; }
        public DateTime? DefNwh_Date { get; set; }
        public int? nwhcatid { get; set; }
    }
}
