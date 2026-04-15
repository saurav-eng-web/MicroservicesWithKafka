namespace MicroservicesWithKafka.Models
{
    public class RoleMaster
    {
        public int? RM_ID { get; set; }
        public string? RM_Name { get; set; }
        public bool? RM_Status { get; set; }
        public bool? AndroidActive { get; set; }
        public bool? IsB2B { get; set; }
    }
}
