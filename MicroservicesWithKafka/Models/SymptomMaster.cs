namespace MicroservicesWithKafka.Models
{
    public class SymptomMaster
    {
        public long? syid { get; set; }           // numeric(18,0)
        public string? SYproduct { get; set; }    // nvarchar(50)
        public string? symptom { get; set; }      // nvarchar(4000)
        public string? status { get; set; }       // nvarchar(50)

        public string? productid { get; set; }       // nvarchar(50)
    }
}
