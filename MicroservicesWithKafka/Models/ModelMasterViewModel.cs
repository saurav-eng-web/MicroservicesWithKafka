using System;

namespace MicroservicesWithKafka.Models
{
    public class ModelMasterViewModel
    {
        public decimal? ModelID { get; set; }
        public decimal? ProductID { get; set; }
        public string ModelNo { get; set; }
        public string Description { get; set; }
        public string ManfBy { get; set; }
        public string PM { get; set; }
        public string CW { get; set; }
        public string NCW { get; set; }
        public string PMMonth { get; set; }
        public string CWMonth { get; set; }
        public string NCWMonth { get; set; }
        public string Status { get; set; } = "INACTIVE";
        public string IsUploaded { get; set; } = "0";
        public DateTime UploadedDateTime { get; set; } = DateTime.Now;
        public decimal? ColorId { get; set; }
        public decimal? CategoryId { get; set; }
        public string ModelCode { get; set; }
        public decimal? SerialNo { get; set; }
        public decimal? FactoryId { get; set; }
        public decimal SerialLimit { get; set; } = 12;
        public decimal? SerialMaxLimit { get; set; }
        public bool? Hi_End { get; set; }
        public bool? Low_end { get; set; }
        public bool? Rate_LT250LFF { get; set; }
        public bool? Rate_GT250_LT450LFF { get; set; }
        public bool? Rate_GT450LFF { get; set; }
        public bool? Ctv14To21 { get; set; }
        public bool? Ctv25To34 { get; set; }
        public decimal? ServCharForRoylty { get; set; }
        public decimal? SubProductID { get; set; }
        public DateTime? ModelLaunchdate { get; set; }
        public DateTime? EndOfDate { get; set; }
        public decimal? Price { get; set; }
        public string HSN { get; set; }
        public decimal? TaxPer { get; set; }
        public decimal? Weight { get; set; }

    }
}
