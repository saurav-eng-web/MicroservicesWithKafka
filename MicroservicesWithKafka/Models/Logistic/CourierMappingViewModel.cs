namespace MicroservicesWithKafka.Models.Logistic
{
    // Translated from CourierMapping.aspx.vb MappingDownload_Click query result
    public class CourierMappingViewModel
    {
        public string CourierCode   { get; set; } = string.Empty;
        public string CourierName   { get; set; } = string.Empty;
        public string Pincode       { get; set; } = string.Empty;
        public string Slab          { get; set; } = string.Empty;
        public string Priority      { get; set; } = string.Empty;
        public string Date          { get; set; } = string.Empty;
        /// <summary>Only filled for the "not mapped" list (CourMappingUpload.Action), as in legacy MappingNotDownload_Click.</summary>
        public string Action        { get; set; } = string.Empty;
    }

    // Translated from CourierMappingUpload table columns used in SqlBulkCopy
    public class CourierMappingUploadViewModel
    {
        public string CourierCode { get; set; } = string.Empty;
        public string Pincode     { get; set; } = string.Empty;
        public string Slab        { get; set; } = string.Empty;
        public string Priority    { get; set; } = string.Empty;
        public string Action      { get; set; } = string.Empty;
    }
}
