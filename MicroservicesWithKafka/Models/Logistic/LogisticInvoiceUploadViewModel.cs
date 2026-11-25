namespace MicroservicesWithKafka.Models.Logistic
{
    public class LogisticInvoicePoUploadViewModel
    {
        public string InvoiceNo { get; set; } = string.Empty;
        public string FromA { get; set; } = string.Empty;
        public string Shipto { get; set; } = string.Empty;
        public string TransporterName { get; set; } = string.Empty;
        public string DocketNo { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
    }

    public class LogisticInvoiceRateUploadViewModel
    {
        public string FromA { get; set; } = string.Empty;
        public string ToA { get; set; } = string.Empty;
        public string Transporter { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string Rate { get; set; } = string.Empty;
    }
}
