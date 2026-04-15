namespace MicroservicesWithKafka.Models
{
    public class SfEngineerResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public IEnumerable<dynamic> Data { get; set; }
    }

}
