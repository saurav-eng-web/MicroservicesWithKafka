using System;

namespace MicroservicesWithKafka.Models
{
    public class ModelVariant
    {
        public decimal? VariantID { get; set; }
        public decimal? ModelID { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Attribute1 { get; set; }
        public string Attribute2 { get; set; }
        public string Attribute3 { get; set; }
        public string Attribute4 { get; set; }
        public string Attribute5 { get; set; }
    }


}
