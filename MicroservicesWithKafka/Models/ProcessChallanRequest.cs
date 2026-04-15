namespace MicroservicesWithKafka.Models
{
    public class ProcessChallanRequest
    {
        public string Type { get; set; }
        public string Warehouse { get; set; }
        public string CourierCode { get; set; }
        public string Doc { get; set; }
        public string Remark { get; set; }
        public string Dimensions { get; set; }
        public string Weight { get; set; }
        public string Box { get; set; }
        public string Ids { get; set; }          // comma-separated IDs
        public string IPAddress { get; set; }
        public string Username { get; set; }
    }

}
