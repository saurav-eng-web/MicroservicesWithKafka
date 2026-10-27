namespace MicroservicesWithKafka.Models.Logistic
{
    public class DocketDownloadViewModel
    {
        public string CourierCode { get; set; } = string.Empty;
        public string Courier { get; set; } = string.Empty;
        public string DocketNo { get; set; } = string.Empty;
        public string IsUsed { get; set; } = string.Empty;
        public string InventoryType { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }

    public class DocketNotUploadedViewModel
    {
        public string CourierCode { get; set; } = string.Empty;
        public string DocketNo { get; set; } = string.Empty;
        public string IsUpload { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }

    public class DocketUploadRequestViewModel
    {
        public string CourierCode { get; set; } = string.Empty;
        public string DocketNo { get; set; } = string.Empty;
    }
}
