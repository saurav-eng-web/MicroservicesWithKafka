namespace MicroservicesWithKafka.Models
{
    public class DocumentUpload
    {
        public int DocumentID { get; set; }
        public string Ticket_Key { get; set; }

        public string Ticket_no { get; set; }
        public string DocumentURL { get; set; }
        public string DocumentType { get; set; }
        public DateTime? TDATE { get; set; }
        public string Path1 { get; set; }
        public string PartCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Status { get; set; }

        // Computed fields from query
        public int RowNum { get; set; }
        public string EnteredDate { get; set; }
        public string DocumentURL1 { get; set; }
    }
}
