namespace MicroservicesWithKafka.Models.Logistic
{
    /// <summary>
    /// Row of the legacy CN Logistic uploader Excel (Logistic/CNLogistic.xlsx, Sheet1):
    /// CNno, dispatchWarehouse, docketNo, dispatchstatus -> table CNLogisticUploader.
    /// </summary>
    public class CnLogisticUploadViewModel
    {
        public string CNno { get; set; } = string.Empty;
        public string DispatchWarehouse { get; set; } = string.Empty;
        public string DocketNo { get; set; } = string.Empty;
        public string DispatchStatus { get; set; } = string.Empty;
    }
}
