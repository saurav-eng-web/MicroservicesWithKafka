namespace MicroservicesWithKafka.Models
{
    public class CourierMappingViewModel
    {
        public string CourierCode   { get; set; } = string.Empty;
        public string CourierName   { get; set; } = string.Empty;
        public string Pincode       { get; set; } = string.Empty;
        public string Slab          { get; set; } = string.Empty;
        public string Priority      { get; set; } = string.Empty;
        public string Date          { get; set; } = string.Empty;
        public string Action        { get; set; } = string.Empty;
    }

    public class CourierMappingUploadViewModel
    {
        public string CourierCode { get; set; } = string.Empty;
        public string Pincode     { get; set; } = string.Empty;
        public string Slab        { get; set; } = string.Empty;
        public string Priority    { get; set; } = string.Empty;
        public string Action      { get; set; } = string.Empty;
    }
}
