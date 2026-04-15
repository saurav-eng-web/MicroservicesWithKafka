namespace MicroservicesWithKafka.Models
{
    public class CallTransferRequest
    {
        public string TicketNo { get; set; }
        public int ToSfId { get; set; }
        public int TransferBy { get; set; }
        public string TransferTo { get; set; }
    }

}
