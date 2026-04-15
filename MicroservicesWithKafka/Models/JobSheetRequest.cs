namespace MicroservicesWithKafka.Models
{
    public class JobSheetRequest
    {
        public string? TicketKey { get; set; }

        public string TICKET_NO { get; set; }
        public string Apptdate { get; set; }
        public string Appttime { get; set; }
        public string? DOP { get; set; }
        public int? Reallot { get; set; }
        public string Type { get; set; }
        public string Type1 { get; set; }
        public string ContactPersion { get; set; }
        public string CustomerRemark { get; set; }
        public int UsrId { get; set; }
        public int? Eng_Id { get; set; }
        public int CallTypeID { get; set; }
        public string CallType { get; set; }
        public string? Action { get; set; }
    }
}
