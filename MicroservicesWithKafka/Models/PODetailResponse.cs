namespace MicroservicesWithKafka.Models
{
    public class PODetailResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public dynamic? Data { get; set; }
    }
}
