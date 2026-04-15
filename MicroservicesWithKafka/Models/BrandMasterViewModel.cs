using System.ComponentModel.DataAnnotations;

namespace MicroservicesWithKafka.Models
{
    public class BrandMasterViewModel
    {
        public decimal? Brand_ID { get; set; }

        [StringLength(50)]
        public string? Brand_Name { get; set; }

        [StringLength(50)]
        public string? Brand_Contact { get; set; }

        public bool? RegCC { get; set; } = false;

        public bool? RegSF { get; set; } = false;

        public bool? Active { get; set; } = true;

        [StringLength(50)]
        public string? ComplaintStart { get; set; }
    }
}
