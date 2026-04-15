namespace MicroservicesWithKafka.Models
{
    public class CallTypeMaster
    {
        public decimal CallType_ID { get; set; }   // numeric(18,0) -> long
        public string? CallType_Name { get; set; }   // nvarchar(100) -> string
        public string? CallType_Active { get; set; } // nchar(10) -> string
        public string? AP_EP { get; set; }           // nvarchar(50) -> string
    }
}
