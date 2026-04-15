using System;
using System.ComponentModel.DataAnnotations;

namespace MicroservicesWithKafka.Models
{
    public class ProductMasterModel
    {
        public decimal? ProductID { get; set; }

        [StringLength(100)]
        public string? Product { get; set; }

        [StringLength(50)]
        public string? ProductCode { get; set; }

        [StringLength(50)]
        public string? Prd_Category { get; set; }

        public decimal? Prd_ModelID { get; set; }

        public decimal? Prd_colorID { get; set; }

        public decimal? Prd_FactoryId { get; set; }

        public decimal? Prd_CategoryID { get; set; }

        [StringLength(500)]
        public string? Prd_WtyDetail1 { get; set; }

        [StringLength(500)]
        public string? Prd_WtyDetail2 { get; set; }

        public decimal? Prd_symptomID { get; set; }

        public decimal? Prd_causeid { get; set; }

        public decimal? Prd_AnlysisId { get; set; }

        [StringLength(1)]
        public string? Isuploaded { get; set; } = "0";

        public DateTime? UploadedDateTime { get; set; }

        [StringLength(15)]
        public string? Status { get; set; }

        [StringLength(15)]
        public string? AMC_Detail { get; set; }

        [StringLength(15)]
        public string? Extwty { get; set; }

        public decimal? BrandId { get; set; }

        public bool? IsEP { get; set; }
    }
}
