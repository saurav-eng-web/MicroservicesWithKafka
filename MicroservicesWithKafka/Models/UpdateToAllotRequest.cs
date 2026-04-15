namespace MicroservicesWithKafka.Models
{
    public class UpdateToAllotRequest
    {
        public string CallNumber { get; set; }
        public string EnteredBy { get; set; }
        public int EnteredByID { get; set; }
        public string Remarks { get; set; }
        public string ASC_Code { get; set; }
    }

}
