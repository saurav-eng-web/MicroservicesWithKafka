namespace MicroservicesWithKafka.Models
{
    public class PrintDataRequest
    {
        public string pono { get; set; }
        public string Type { get; set; }      // "invoicedetail" or "ChallanLine"
        public string challantype { get; set; }     // "Defective"
    }

}
