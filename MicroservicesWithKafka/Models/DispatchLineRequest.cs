namespace MicroservicesWithKafka.Models
{
    public class DispatchLineRequest
    {
        public int Sfid { get; set; }
        public int Whs { get; set; }   // Warehouse Id (string because SP expects nvarchar)
        public string Type { get; set; }  // defective
    }
}
