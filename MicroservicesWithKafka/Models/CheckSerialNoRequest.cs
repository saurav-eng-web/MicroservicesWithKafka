namespace MicroservicesWithKafka.Models
{
    public class CheckSerialNoRequestModel
    {
        public string SerialNo { get; set; }
        public string TicketNo { get; set; }
        public int? Pincode { get; set; }   // optional
        public string Type { get; set; }    // WIPProcess / simpcheck / partcodeserialno / DOASerialNo
    }
}
