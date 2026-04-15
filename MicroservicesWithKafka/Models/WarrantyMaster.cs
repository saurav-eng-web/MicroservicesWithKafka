namespace MicroservicesWithKafka.Models
{
    public class WarrantyMaster
    {
        public long? Warranty_ID { get; set; }             // numeric(18,0) → long
        public string? Warranty_Desc { get; set; }        // nvarchar(100)
        public string? Warranty_Active { get; set; }      // nchar(10)
        public string? Warranty_Type { get; set; }        // nvarchar(100)
    }
}
