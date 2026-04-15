namespace MicroservicesWithKafka.Models
{
    public class SFReceivePORequest
    {
        public string Action { get; set; }

        public decimal? ID { get; set; }
        public string? PONumber { get; set; }
        public decimal? POMasterID { get; set; }
        public decimal? PODetailID { get; set; }
        public decimal? ModelID { get; set; }
        public string? PartCode { get; set; }
        public string? PartDesc { get; set; }

        public decimal? DispatchQty { get; set; }
        public decimal? OKQty { get; set; }
        public decimal? DamageQty { get; set; }
        public decimal? FaultyQty { get; set; }
        public decimal? WrongQty { get; set; }
        public decimal? SFID { get; set; }

        public decimal? MiscBinQty { get; set; }
        public decimal? MoveToOkBin { get; set; }
        public decimal? defective { get; set; }
        public decimal? freshfaulty { get; set; }

        public string? challanNo { get; set; }
        public string? consumption_ticketno { get; set; }
    }
}
