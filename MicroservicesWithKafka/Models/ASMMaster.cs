namespace MicroservicesWithKafka.Models
{
    public class ASMMaster
    {
        public int ASM_ID { get; set; }
        public string? ASM_Name { get; set; }
        public int IsActive { get; set; }
        public DateTime EnteredDate { get; set; }
        public string? EnteredBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? ContactNo { get; set; }
        public DateTime? DOJ { get; set; }

        public int RSM_ID { get; set; }
        public string? RSM_Name { get; set; }

    }
}
