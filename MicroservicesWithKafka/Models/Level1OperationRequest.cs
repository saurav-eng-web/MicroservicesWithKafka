namespace MicroservicesWithKafka.Models
{
    public class Level1OperationRequest
    {
        public string TicketNo { get; set; }
        public string UsrName { get; set; }
        public string Type { get; set; }
        public string Apptdate { get; set; }
        public string Appttime { get; set; }
        public string? DOP { get; set; }
        public string? Reallot { get; set; }
        public string ContactPersion { get; set; }
        public string CustomerRemark { get; set; }
        public string UsrId { get; set; }
        public string? Eng_id { get; set; }
        public string CallTypeID { get; set; }
        public string CALLTYPE { get; set; }
    }
}
